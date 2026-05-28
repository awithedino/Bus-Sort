using System.Collections.Generic;
using UnityEngine;

public class PassengerPool : MonoBehaviour
{
    public static PassengerPool Instance;
    
    public PassengerGroup passengerPrefab;
    private Queue<PassengerGroup> pool = new Queue<PassengerGroup>();

    void Awake() 
    { 
        Instance = this; 
    }

    public PassengerGroup GetPassenger(Vector3 position, Quaternion rotation, Transform parent)
    {
        if (pool.Count > 0)
        {
            PassengerGroup p = pool.Dequeue();
            p.transform.SetPositionAndRotation(position, rotation);
            p.transform.SetParent(parent);
            p.gameObject.SetActive(true);
            return p;
        }
        return Instantiate(passengerPrefab, position, rotation, parent);
    }

    public void ReturnPassenger(PassengerGroup passenger)
    {
        passenger.gameObject.SetActive(false);
        pool.Enqueue(passenger);
    }
}