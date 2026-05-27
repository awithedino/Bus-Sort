using UnityEngine;
using UnityEngine.Splines;

public class SplineQueueManager : MonoBehaviour
{
    public SplineContainer loopSpline;
    public int totalSpots = 12;
    
    // Track the slot
    private PassengerGroup[] occupiedSlots;
    private float slotSpacing;

    void Start()
    {
        occupiedSlots = new PassengerGroup[totalSpots];
        slotSpacing = 1f / totalSpots;
    }

    public bool CanSpawnAtSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= totalSpots) return false;
        return occupiedSlots[slotIndex] == null;
    }

    public void SpawnPassengerAtSlot(PassengerGroup newGroup, int slotIndex)
    {
        occupiedSlots[slotIndex] = newGroup;
        float targetT = slotIndex * slotSpacing;
        newGroup.InitToTrack(loopSpline, targetT);
        
        // Shift check if the slots are empty
        ShiftPassengers();
    }

    public void ShiftPassengers()
    {
        bool someoneMoved = false;
        for (int i = 1; i < totalSpots; i++)
        {
            if (occupiedSlots[i] != null && occupiedSlots[i - 1] == null)
            {
                occupiedSlots[i - 1] = occupiedSlots[i];
                occupiedSlots[i] = null;
                
                float targetT = (i - 1) * slotSpacing;
                occupiedSlots[i - 1].MoveAlongSpline(targetT);
                someoneMoved = true;
            }
        }

        if (someoneMoved) Invoke(nameof(ShiftPassengers), 0.4f);
    }

    public void ReleaseGatePassenger(Transform carDoor)
    {
        if (occupiedSlots[0] != null)
        {
            occupiedSlots[0].BoardCar(carDoor);
            occupiedSlots[0] = null;
            
            ShiftPassengers();
        }
    }
    
}
