using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

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
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            
            Ray ray = mainCam.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector2Int gridPos = gridManager.WorldToGrid(hit.point);
                Vehicle clickedVehicle = gridManager.GetVehicleAt(gridPos.x, gridPos.y);
                if (clickedVehicle != null)
                {
                    VehicleAnimator carAnim = clickedVehicle.GetComponent<VehicleAnimator>();
                    if (carAnim.isAnimating) return; 
                    
                    if (gridManager.perimeterTrack.CheckMathEscapeClear(clickedVehicle, gridManager, out Vector3 exitPos, out int bumpSpaces)) 
                    {
                        Transform targetSlot = gridManager.parkingManager.GetNextAvailableSlot(clickedVehicle);
                        
                        if (targetSlot != null) {
                            gridManager.RemoveVehicleFromGrid(clickedVehicle);
                            gridManager.perimeterTrack.RouteEscapingVehicle(clickedVehicle, exitPos, targetSlot);
                        } else {
                            gridManager.ShakeVehicle(clickedVehicle); // Grid full!
                        }
                    } 
                    else 
                    {
                        // Blocked!
                        gridManager.BumpVehicle(clickedVehicle, bumpSpaces);
                    }
                }
            }
        }
    }
}