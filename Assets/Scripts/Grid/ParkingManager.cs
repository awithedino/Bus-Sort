using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class ParkingManager : MonoBehaviour
{
    public Transform[] parkingSlots;
    public List<Vehicle> parkedCars = new();

    public Transform GetNextAvailableSlot(Vehicle carToPark)
    {
        if (parkedCars.Count < parkingSlots.Length)
        {
            var slot = parkingSlots[parkedCars.Count];
            parkedCars.Add(carToPark);
            return slot;
        }
        Debug.Log("The loading zone is full! 🛑");
        return null;
    }

    public Vehicle FindMatchingCar(ColorType requestedColor)
    {
        foreach (var car in parkedCars)
            if (car.carColor == requestedColor && !car.isFull())
                return car;

        return null;
    }
    
    public void DispatchCar(Vehicle fullCar)
    {
        if (!parkedCars.Contains(fullCar)) return;
        parkedCars.Remove(fullCar);

        // Exit animation
        var leaveSeq = DOTween.Sequence();
        leaveSeq.Append(fullCar.transform.DOMoveZ(fullCar.transform.position.z - 2f, 0.4f).SetEase(Ease.OutQuad));
        leaveSeq.Append(fullCar.transform.DOMoveX(fullCar.transform.position.x - 20f, 1f).SetEase(Ease.InBack));
        leaveSeq.OnComplete(() => Destroy(fullCar.gameObject));

        ShiftCars();
    }

    private void ShiftCars()
    {
        for (var i = 0; i < parkedCars.Count; i++)
            parkedCars[i].transform.DOMove(parkingSlots[i].position, 0.5f).SetEase(Ease.OutQuad);
    }
}