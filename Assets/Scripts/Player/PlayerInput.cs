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
                    
                    if (carAnim.CheckEscapeClear(out Vector3 frameHit, out float bumpDistance)) 
                    {
                        bool actuallyEscaped = gridManager.EscapeVehicle(clickedVehicle, frameHit);

                        if (actuallyEscaped) {
                            Debug.Log($"Vehicle {clickedVehicle.id} is out.");
                        } else {
                            Debug.Log($"Vehicle {clickedVehicle.id} path clear, but loading zone is full.");
                        }
                    } 
                    else 
                    {
                        int spaces = Mathf.FloorToInt(bumpDistance);
                        gridManager.BumpVehicle(clickedVehicle, spaces);
                        Debug.Log($"Vehicle {clickedVehicle.id} is blocked.");
                    }
                }
            }
        }
    }
}