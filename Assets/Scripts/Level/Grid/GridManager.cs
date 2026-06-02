using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Level References")]
    public Transform vehicleParent;
    public ParkingManager parkingManager;
    public PerimeterTrack perimeterTrack;

    [Header("Active Vehicles")]
    public List<Vehicle> activeVehicles = new List<Vehicle>();
    public void RegisterVehicle(Vehicle newVehicle) 
    {
        if (!activeVehicles.Contains(newVehicle))
        {
            activeVehicles.Add(newVehicle);
        }
    }
    
    public void RemoveVehicle(Vehicle car)
    {
        if (activeVehicles.Contains(car))
        {
            activeVehicles.Remove(car);
        }
    }
    
    public void ClearBoard()
    {
        Vehicle[] leftoverCars = vehicleParent.GetComponentsInChildren<Vehicle>(true);
        
        foreach (Vehicle car in leftoverCars)
        {
            Destroy(car.gameObject);
        }
    }
}