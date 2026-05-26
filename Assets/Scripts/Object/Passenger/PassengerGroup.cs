using UnityEngine;
using DG.Tweening;
using UnityEngine.Splines;

public class PassengerGroup : MonoBehaviour
{
    public ColorType groupColor;
    public Transform[] passengers;

    [HideInInspector] public float currentT = 1f;
    private SplineContainer targetTrack;

    public void InitToTrack(SplineContainer track, float startT)
    {
        targetTrack = track;
        currentT = startT;
        transform.position = targetTrack.EvaluatePosition(currentT);
    }

    public void MoveAlongSpline(float targetT)
    {
        DOTween.To(() => currentT, x => {
            currentT = x;
            if (targetTrack != null) {
                transform.position = targetTrack.EvaluatePosition(currentT);
                transform.forward = targetTrack.EvaluateTangent(currentT);
            }
        }, targetT, 0.35f).SetEase(Ease.InOutQuad);
    }

    public void BoardCar(Transform carDoor)
    {
        for (int i = 0; i < passengers.Length; i++)
        {
            float jumpDelay = i * 0.15f;

            passengers[i].DOJump(carDoor.position, 0.5f, 1, 0.4f)
                .SetDelay(jumpDelay)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    // Hide when the passengers are on the car
                    passengers[i].gameObject.SetActive(false);  
                });
        }
    }
}
