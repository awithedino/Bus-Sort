using UnityEngine;
using DG.Tweening;

public class PlayerInput : MonoBehaviour
{
    public GridManager gridManager;
    private Camera mainCam;

    void Start() 
    {
        mainCam = Camera.main; // Caching this is better for performance!
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
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