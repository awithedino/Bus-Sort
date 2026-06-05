using UnityEngine;
using System.Collections;
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
    
    public void CheckForDeadlock() 
    {
        Debug.Log("--- Checking for Deadlock ---");
        
        // 1. Check for empty slots
        foreach (Vehicle car in parkedCars) 
        {
            if (car == null) 
            {
                Debug.Log("Safe: Found an empty parking slot!");
                return; 
            }
        }
        Debug.Log("All slots are full...");

        // 2. Check for full cars
        foreach (Vehicle car in parkedCars)
        {
            if (car.isFull()) 
            {
                Debug.Log($"Safe: Car {car.gameObject.name} is flagged as full and about to leave!");
                return; 
            }
        }
        Debug.Log("No cars are full...");

        // 3. Check for matching passengers
        SplineQueueManager queueManager = FindAnyObjectByType<SplineQueueManager>();
        if (queueManager != null)
        {
            foreach (Vehicle car in parkedCars)
            {
                foreach (PassengerGroup group in queueManager.loopQueue)
                {
                    if (group.currentTrack != null && group.currentTrack.Spline.Closed)
                    {
                        if (group.myColor == car.carColor) 
                        {
                            Debug.Log($"Safe: Found a {group.myColor} passenger matching a parked car!");
                            return; 
                        }
                    }
                }
            }
        }
        Debug.Log("No matching passengers found on the loop...");

        // 4. Fire the Game Over!
        Debug.Log("DEADLOCK CONFIRMED! Sending Game Over in 0.6s 💀");
        StartCoroutine(TriggerGameOverDelayed());
    }

    private IEnumerator TriggerGameOverDelayed()
    {
        yield return new WaitForSeconds(0.6f);
        GameManager.Instance.GameOver();
    }
}