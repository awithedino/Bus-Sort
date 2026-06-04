using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public GridManager gridManager; 
    private Camera mainCam;

    void Start() 
    {
        mainCam = Camera.main; 
    }

    void Update()
    {
        // Click on nothing
        if (Pointer.current == null || !Pointer.current.press.wasPressedThisFrame) return;

        // Clicked the sky/nothing
        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Ray ray = mainCam.ScreenPointToRay(screenPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        // Not a car/car is already parked
        Vehicle clickedVehicle = hit.collider.GetComponentInParent<Vehicle>();
        if (clickedVehicle == null || clickedVehicle.isParked) return;
        
        // Car is driving
        VehicleAnimator carAnim = clickedVehicle.GetComponent<VehicleAnimator>();
        if (carAnim.isAnimating) return; 
        
        // Logic
        if (gridManager.perimeterTrack.CheckFreeformEscapeClear(clickedVehicle, out float blockDistance, out Vector3 frameHitPoint, out Vehicle hitCar)) 
        {
            Transform targetSlot = gridManager.parkingManager.GetNextAvailableSlot(clickedVehicle);
            
            if (targetSlot != null) 
            {
                gridManager.RemoveVehicle(clickedVehicle);
                gridManager.perimeterTrack.RouteEscapingVehicle(clickedVehicle, frameHitPoint, targetSlot);
            } 
            else 
            {
                carAnim.AnimateShake(); 
            }
        } 
        else 
        {
            carAnim.AnimateBump(blockDistance); 
            if (hitCar != null) 
            {
                hitCar.GetComponent<VehicleAnimator>().AnimateWobble(clickedVehicle.transform.forward);
            }
        }
    }
}