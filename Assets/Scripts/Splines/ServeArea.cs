using UnityEngine;
using DG.Tweening;

public class ServeArea : MonoBehaviour
{
    public SplineQueueManager mainQueue;
    public ParkingManager parkingManager; 

    private void OnTriggerEnter(Collider other)
         {
             PassengerGroup passenger = other.GetComponent<PassengerGroup>();
             if (passenger == null) return;
             Vehicle targetCar = parkingManager.FindMatchingCar(passenger.myColor);
             
             if (targetCar != null)
             {
                 BoardCar(passenger, targetCar);
             }
         }

    private void BoardCar(PassengerGroup passenger, Vehicle car)
    {
        car.currentCapacity += 4; 
        passenger.currentTrack = null; 
        mainQueue.RemovePassengerFromLine(passenger);
        
        passenger.transform.DOMove(car.transform.position, 0.5f).SetEase(Ease.InOutQuad).OnComplete(() => 
        {
            Destroy(passenger.gameObject);
            if (car.isFull()) 
            {
                parkingManager.DispatchCar(car);
            }
        });
    }
}