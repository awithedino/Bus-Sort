using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ParkingManager : MonoBehaviour {
    public Transform[] parkingSlots; 
    public List<Vehicle> parkedCars = new List<Vehicle>();
    
    public Transform GetNextAvailableSlot(Vehicle carToPark) {
        if (parkedCars.Count < parkingSlots.Length) {
            Transform slot = parkingSlots[parkedCars.Count];
            parkedCars.Add(carToPark); 
            return slot;
        }
        
        Debug.Log("The loading zone is full! 🛑");
        return null;
    }

    void Update() {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) {
            RemoveFirstCar();
        }
    }

    public void RemoveFirstCar() {
        if (parkedCars.Count == 0) return;

        Vehicle carToLeave = parkedCars[0];
        parkedCars.RemoveAt(0);
        
        // TODO: Object Pooling
        Destroy(carToLeave.gameObject);
        ShiftCars();
    }   

    private void ShiftCars() {
        for (int i = 0; i < parkedCars.Count; i++) {
            // For now, they instantly snap to the next slot over
            parkedCars[i].transform.position = parkingSlots[i].position;
        }
    }
}