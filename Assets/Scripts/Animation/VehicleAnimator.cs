using UnityEngine;
using DG.Tweening;

public class VehicleAnimator : MonoBehaviour {
    public bool isAnimating = false;
    
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

    public void AnimateEscape(Direction moveDir, Transform targetSlot) {
        isAnimating = true;
        
        Vector3 startPos = transform.position;
        Vector3 burstPos = startPos; 
        float burstDist = 2.0f; 
        
        switch (moveDir) {
            case Direction.Up: burstPos.z += burstDist; break;
            case Direction.Down: burstPos.z -= burstDist; break;
            case Direction.Right: burstPos.x += burstDist; break;
            case Direction.Left: burstPos.x -= burstDist; break;
        }
        Vector3 finalPos = targetSlot.position;
        finalPos.y = startPos.y;

        Vector3[] path = new Vector3[] { startPos, burstPos, finalPos };
        transform.DOPath(path, 0.6f, PathType.CatmullRom)
            .SetEase(Ease.OutQuad)
            .SetLookAt(0.01f, Vector3.forward, Vector3.up) 
            .OnComplete(() => {
                transform.rotation = Quaternion.Euler(0, targetSlot.rotation.eulerAngles.y, 0);
                isAnimating = false;
                GetComponent<Vehicle>().isParked = true; 
            });
    }

    public void AnimateLeave() {
        Sequence leaveSeq = DOTween.Sequence();
        leaveSeq.Append(transform.DOMoveZ(transform.position.z - 1.5f, 0.3f).SetEase(Ease.OutQuad));
        leaveSeq.Append(transform.DOMoveX(transform.position.x - 20f, 0.8f).SetEase(Ease.InBack));
        leaveSeq.OnComplete(() => Destroy(gameObject));
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