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

    public void AnimateEscape(Transform targetSlot, Vector3 gridExitPoint)
    {
        isAnimating = true;
        if (myCollider != null) myCollider.enabled = false;

        float flatY = transform.position.y;
        Vector3 flatExit = new Vector3(gridExitPoint.x, flatY, gridExitPoint.z);
        Vector3 lanePos = targetSlot.position + (targetSlot.forward * -4f);
        Vector3 flatSlot = new Vector3(targetSlot.position.x, flatY, targetSlot.position.z);
        
        Vector3[] approachPath = new Vector3[] { transform.position, flatExit, lanePos };
        Sequence parkSeq = DOTween.Sequence();
        
        parkSeq.Append(transform.DOPath(approachPath, 0.6f, PathType.CatmullRom)
            .SetOptions(false, AxisConstraint.Y)
            .SetLookAt(0.01f)
            .SetEase(Ease.InOutQuad));
        parkSeq.Append(transform.DOMove(flatSlot, 0.2f).SetEase(Ease.OutQuad));
        
        Vector3 targetRot = new Vector3(0, targetSlot.rotation.eulerAngles.y, 0);
        parkSeq.Join(transform.DORotate(targetRot, 0.2f));

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