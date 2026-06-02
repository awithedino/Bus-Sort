using System.Collections.Generic;
using UnityEngine;

public class PerimeterTrack : MonoBehaviour
{
    [Header("Escape Route")]
    public Transform[] escapeGates;

    public bool CheckMathEscapeClear(Vehicle car, GridManager grid, out Vector3 exitWorldPos, out int bumpSpaces)
    {
        exitWorldPos = Vector3.zero;
        bumpSpaces = 0;
        
        int stepX = 0, stepY = 0;
        switch (car.moveDirection) {
            case Direction.Up: stepY = 1; break;
            case Direction.Down: stepY = -1; break;
            case Direction.Left: stepX = -1; break;
            case Direction.Right: stepX = 1; break;
        }

        int headX = -1, headY = -1;
        for (int x = 0; x < grid.width; x++) {
            for (int y = 0; y < grid.height; y++) {
                if (grid.gridState[x, y] == car.id) {
                    if (headX == -1) { headX = x; headY = y; }
                    else {
                        if (stepX > 0 && x > headX) headX = x;
                        if (stepX < 0 && x < headX) headX = x;
                        if (stepY > 0 && y > headY) headY = y;
                        if (stepY < 0 && y < headY) headY = y;
                    }
                }
            }
        }

        int currX = headX + stepX;
        int currY = headY + stepY;

        while (currX >= 0 && currX < grid.width && currY >= 0 && currY < grid.height)
        {
            if (grid.gridState[currX, currY] != 0 && grid.gridState[currX, currY] != car.id) {
                return false; 
            }
            bumpSpaces++;
            currX += stepX;
            currY += stepY;
        }

        exitWorldPos = grid.GridToWorld(currX, currY); 
        return true;
    }
    
    public void RouteEscapingVehicle(Vehicle car, Vector3 exitWorldPos, Transform targetSlot)
    {
        Transform closestGate = escapeGates[0];
        float minDist = Vector3.Distance(exitWorldPos, closestGate.position);
        
        foreach (var gate in escapeGates) {
            float dist = Vector3.Distance(exitWorldPos, gate.position);
            if (dist < minDist) {
                minDist = dist;
                closestGate = gate;
            }
        }

        List<Vector3> escapeRoute = new List<Vector3> { exitWorldPos, closestGate.position, targetSlot.position };
        car.GetComponent<VehicleAnimator>().AnimateEscape(escapeRoute, targetSlot);
        car.arrowVisual.gameObject.SetActive(false);
    }
    
}