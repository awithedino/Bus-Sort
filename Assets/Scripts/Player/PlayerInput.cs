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
                    bool canEscape = gridManager.CheckPath(clickedVehicle, gridPos.x, gridPos.y, out int spaces);

                    if (canEscape) {
                        bool actuallyEscaped = gridManager.EscapeVehicle(clickedVehicle);

                        if (actuallyEscaped) {
                            Debug.Log($"Vehicle {clickedVehicle.id} is out.");
                        } else {
                            Debug.Log($"Vehicle {clickedVehicle.id} path clear, but loading zone is full.");
                        }
                    } else {
                        gridManager.BumpVehicle(clickedVehicle, spaces);
                        Debug.Log($"Vehicle {clickedVehicle.id} is blocked.");
                    }
                }
            }
        }
    }
}