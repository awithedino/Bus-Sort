using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public GridManager gridManager; // Keep the name for now so your Inspector doesn't break!
    private Camera mainCam;

    void Start() 
    {
        mainCam = Camera.main; 
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            Ray ray = mainCam.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vehicle clickedVehicle = hit.collider.GetComponentInParent<Vehicle>();
                if (clickedVehicle != null)
                {
                    VehicleAnimator carAnim = clickedVehicle.GetComponent<VehicleAnimator>();
                    if (carAnim.isAnimating) return; 
                    if (gridManager.perimeterTrack.CheckFreeformEscapeClear(clickedVehicle, out float blockDistance, out Vector3 frameHitPoint, out Vehicle hitCar)) 
                    {
                        Transform targetSlot = gridManager.parkingManager.GetNextAvailableSlot(clickedVehicle);
                        
                        if (targetSlot != null) {
                            gridManager.RemoveVehicle(clickedVehicle);
                            gridManager.perimeterTrack.RouteEscapingVehicle(clickedVehicle, frameHitPoint, targetSlot);
                        } else {
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
        }
    }
}   