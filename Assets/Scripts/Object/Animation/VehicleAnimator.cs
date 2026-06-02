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
    
    public void AnimateBump(float distanceToObstacle) {
        if (isAnimating) return; 
        isAnimating = true;

        Vector3 originalPos = transform.position;
        float driveDistance = Mathf.Max(0.1f, distanceToObstacle - 0.4f); 
        
        Vector3 targetPos = originalPos + (transform.forward * driveDistance);
        
        Sequence bumpSeq = DOTween.Sequence();
        bumpSeq.Append(transform.DOMove(targetPos, 0.2f).SetEase(Ease.OutQuad)); 
        bumpSeq.Append(transform.DOMove(originalPos, 0.25f).SetEase(Ease.InOutSine));
        bumpSeq.OnComplete(() => isAnimating = false);
    }

    public void AnimateEscape(List<Vector3> escapePath, Transform targetSlot)
    {
        isAnimating = true;
        if (myCollider != null) myCollider.enabled = false;
        if (myVehicle.arrowVisual != null) myVehicle.arrowVisual.gameObject.SetActive(false);

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
        Vector3 targetRot = new Vector3(0, targetSlot.rotation.eulerAngles.y - 90f, 0);
        parkSeq.Join(transform.DORotate(targetRot, 0.25f));

        parkSeq.OnComplete(() => {
            isAnimating = false;
            myVehicle.isParked = true; 
            myVehicle.SetTextVisible(true);
            if (myVehicle.capacityCanvas != null) myVehicle.capacityCanvas.gameObject.SetActive(true);
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