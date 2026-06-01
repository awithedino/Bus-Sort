using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VehicleAnimator : MonoBehaviour {
    public bool isAnimating = false;
    private Vehicle myVehicle; 
    private Collider myCollider;
    
    void Awake()
    {
        myVehicle = GetComponent<Vehicle>();
        myCollider = GetComponent<Collider>();
    }
    
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
    
    public bool CheckEscapeClear(out Vector3 hitPoint, out float bumpDistance) 
    {
        hitPoint = Vector3.zero;
        bumpDistance = 0f;
        
        BoxCollider box = myCollider as BoxCollider;
        if (box == null) return false; // Failsafe

        Vector3 localHalfExtents = box.size * 0.5f * 0.8f; 
        Vector3 exactCenter = transform.TransformPoint(box.center);
        
        int combinedLayer = LayerMask.GetMask("Interactable", "PuzzleBoard");
        if (Physics.BoxCast(exactCenter, localHalfExtents, transform.forward, out RaycastHit hit, transform.rotation, 50f, combinedLayer)) 
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable")) 
            {
                bumpDistance = hit.distance; 
                return false;
            }

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("PuzzleBoard")) 
            {
                hitPoint = hit.point;
                return true;
            }
        }
        return false;
    }
    
    public void AnimateBump(Direction dir, int spaces) {
        if (isAnimating) return; 
        isAnimating = true;
        
        Vector3 moveVec = Vector3.zero;
        switch(dir) {
            case Direction.Up: moveVec = new Vector3(0, 0, 1); break;
            case Direction.Down: moveVec = new Vector3(0, 0, -1); break;
            case Direction.Left: moveVec = new Vector3(-1, 0, 0); break;
            case Direction.Right: moveVec = new Vector3(1, 0, 0); break;
        }

        Vector3 originalPos = transform.position;
        float driveDistance = spaces > 0 ? spaces : spaces + 0.15f; 
        Vector3 targetPos = originalPos + (moveVec * driveDistance);
        
        Sequence bumpSeq = DOTween.Sequence();
        bumpSeq.Append(transform.DOMove(targetPos, 0.2f).SetEase(Ease.OutQuad)); 
        bumpSeq.Append(transform.DOMove(originalPos, 0.25f).SetEase(Ease.InOutSine));
        bumpSeq.OnComplete(() => isAnimating = false);
    }

    public void AnimateEscape(List<Vector3> escapePath, Transform targetSlot)
    {
        isAnimating = true;
        if (myCollider != null) myCollider.enabled = false;

        float flatY = transform.position.y;
        
        Vector3[] approachPath = new Vector3[escapePath.Count];
        for(int i = 0; i < escapePath.Count; i++) {
            approachPath[i] = new Vector3(escapePath[i].x, flatY, escapePath[i].z);
        }

        Vector3 flatSlot = new Vector3(targetSlot.position.x, flatY, targetSlot.position.z);
        Sequence parkSeq = DOTween.Sequence();
        
        parkSeq.Append(transform.DOPath(approachPath, 1.2f, PathType.Linear)
            .SetOptions(false, AxisConstraint.Y)
            .SetLookAt(0.01f)
            .SetEase(Ease.Linear));
        
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));
        parkSeq.Append(transform.DOMove(flatSlot, 0.25f).SetEase(Ease.OutQuad));
        Vector3 targetRot = new Vector3(0, targetSlot.rotation.eulerAngles.y, 0);
        parkSeq.Join(transform.DORotate(targetRot, 0.25f));

        parkSeq.OnComplete(() => {
            isAnimating = false;
            myVehicle.isParked = true; 
            myVehicle.SetTextVisible(true); 
            if (myCollider != null) myCollider.enabled = true;
        });
    }

    public void AnimateLeave()
    {
        Sequence exitSequence = DOTween.Sequence();
        Vector3 lanePosition = transform.position + (transform.forward * -2f); 
        exitSequence.Append(transform.DOMove(lanePosition, 0.5f).SetEase(Ease.OutBack));
        Vector3 offScreenPosition = lanePosition + (Vector3.right * 15f); 
        exitSequence.Append(transform.DOLookAt(offScreenPosition, 0.1f));
        exitSequence.Append(transform.DOMove(offScreenPosition, 1f).SetEase(Ease.InQuad));

        exitSequence.OnComplete(() => {
            GameManager.Instance.CarEscaped();
            Destroy(gameObject);
        });
    }
    
    public void AnimateShake() {
        if (isAnimating) return;
        isAnimating = true;
        transform.DOShakeRotation(0.3f, new Vector3(0, 25f, 0), 15, 90, false)
                 .OnComplete(() => isAnimating = false);
    }
    
    public void UpdateVisualRotation(Direction dir) {
        Vector3 rotationAngles = Vector3.zero; 

        switch (dir) {
            case Direction.Up: rotationAngles = new Vector3(0, 0, 0); break;
            case Direction.Right: rotationAngles = new Vector3(0, 90, 0); break;
            case Direction.Down: rotationAngles = new Vector3(0, 180, 0); break;
            case Direction.Left: rotationAngles = new Vector3(0, -90, 0); break;
        }

        transform.rotation = Quaternion.Euler(rotationAngles);
    }
}