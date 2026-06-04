using UnityEngine;
using DG.Tweening;

public class StuntPassenger : MonoBehaviour
{
    public Renderer meshRenderer;

    public void JumpIntoCar(Vehicle car, Material mat, float delay, bool isLastPerson)
    {
        meshRenderer.sharedMaterial = mat;
        transform.DOJump(car.transform.position, 1.5f, 1, 0.2f)
            .SetDelay(delay)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => 
            {
                if (car.audioSource != null && car.hopInSound != null)
                {
                    car.audioSource.pitch = Random.Range(0.9f, 1.2f);
                    car.audioSource.PlayOneShot(car.hopInSound);
                }

                car.boardedCapacity += 1;
                car.UpdateCapacityUI();
                
                if (isLastPerson && car.boardedCapacity >= car.capacity)
                {
                    car.SetTextVisible(false);
                    FindAnyObjectByType<ParkingManager>().DispatchCar(car);
                }
                
                StuntPool.Instance.ReturnStunt(this);
            });
    }
}