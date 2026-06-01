using UnityEngine;
using DG.Tweening;

public class ParkingManager : MonoBehaviour 
{
    public Transform[] parkingSlots; 
    public Vehicle[] parkedCars; 

    void Awake()
    {
        parkedCars = new Vehicle[parkingSlots.Length];
    }
    
    public Transform GetNextAvailableSlot(Vehicle carToPark) 
    {
        for (int i = 0; i < parkedCars.Length; i++)
        {
            if (parkedCars[i] == null)
            {
                parkedCars[i] = carToPark;
                return parkingSlots[i];
            }
        }
        
        Debug.Log("The loading zone is full! 🛑");
        return null;
    }

    public Vehicle FindMatchingCar(ColorType requestedColor) 
    {
        foreach (Vehicle car in parkedCars) 
        {
            if (car != null && car.carColor == requestedColor && !car.isFull() && car.isParked) 
            {
                return car; 
            }
        }
        return null; 
    }

    public void DispatchCar(Vehicle fullCar) 
    {
        for (int i = 0; i < parkedCars.Length; i++)
        {
            if (parkedCars[i] == fullCar)
            {
                parkedCars[i] = null;
                break;
            }
        }
        fullCar.GetComponent<VehicleAnimator>().AnimateLeave();
    }   
}