using UnityEngine;
using DG.Tweening;

public class VehicleAnimator : MonoBehaviour {
    public bool isAnimating = false;
    
    public void AnimateBump(Direction dir, int spaces) {
        if (isAnimating) return; // The hard lock!
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
        Vector2 burstPos = transform.position;
        float burstDist = 1.5f; 
        
        switch (moveDir) {
            case Direction.Up: burstPos.y += burstDist; break;
            case Direction.Down: burstPos.y -= burstDist; break;
            case Direction.Right: burstPos.x += burstDist; break;
            case Direction.Left: burstPos.x -= burstDist; break;
        }

        Sequence escapeSeq = DOTween.Sequence();
        escapeSeq.Append(transform.DOMove(burstPos, 0.15f).SetEase(Ease.InFlash));
        escapeSeq.Append(transform.DOMove(targetSlot.position, 0.3f).SetEase(Ease.InOutFlash));  
        escapeSeq.Join(transform.DORotate(targetSlot.rotation.eulerAngles, 0.3f));
    }
    
    public void AnimateShake() {
        if (isAnimating) return;
        isAnimating = true;
        transform.DOShakeRotation(0.3f, new Vector3(0, 25f, 0), 15, 90, false);
    }
    
    public void UpdateVisualRotation(Direction dir) {
        Vector3 rotationAngles = new Vector3(90, 0, 0); 

        switch (dir) {
            case Direction.Up: rotationAngles = new Vector3(90, 0, 0); break;
            case Direction.Right: rotationAngles = new Vector3(90, 90, 0); break;
            case Direction.Down: rotationAngles = new Vector3(90, 180, 0); break;
            case Direction.Left: rotationAngles = new Vector3(90, -90, 0); break;
        }

        transform.rotation = Quaternion.Euler(rotationAngles);
    }
}