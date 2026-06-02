using System.Collections.Generic;
using UnityEngine;

public class PerimeterTrack : MonoBehaviour
{
    [Tooltip("Drag your 8 corner points here IN ORDER (like a clock face)")]
    public List<Transform> waypoints;

    // --- 1. The Smart Border Scanner ---
    public bool CheckFreeformEscapeClear(Vehicle car, out float blockDistance, out Vector3 frameHitPoint)
    {
        blockDistance = 0f;
        frameHitPoint = Vector3.zero;
        Vector3 boxExtents = new Vector3(0.4f, 0.4f, 0.4f); 

        if (Physics.BoxCast(car.transform.position, boxExtents, car.transform.forward, out RaycastHit hitInfo, car.transform.rotation, 50f))
        {
            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("PuzzleBoard"))
            {
                frameHitPoint = car.transform.position + (car.transform.forward * hitInfo.distance);
                return true; 
            }
            
            blockDistance = hitInfo.distance; 
            return false; 
        }

        frameHitPoint = car.transform.position + (car.transform.forward * 15f);
        return true; 
    }

    // --- 2. The Routing Link ---
    public void RouteEscapingVehicle(Vehicle car, Vector3 frameHitPoint, Transform targetSlot)
    {
        List<Vector3> escapeRoute = GetShortestPath(frameHitPoint, targetSlot.position);
        
        // (Optional: turn off your arrow visual if you still have it on the Vehicle script!)
        // if (car.arrowVisual != null) car.arrowVisual.gameObject.SetActive(false);

        car.GetComponent<VehicleAnimator>().AnimateEscape(escapeRoute, targetSlot);
    }

    // --- 3. Your Restored Math Magic ---
    public List<Vector3> GetShortestPath(Vector3 carPos, Vector3 destination)
    {
        if (waypoints == null || waypoints.Count < 3) return null;

        List<Vector3> pts = new List<Vector3>();
        foreach (var wp in waypoints) pts.Add(wp.position);

        int entryIndex = FindNearestSegment(carPos, pts);
        Vector3 entryPoint = ProjectOntoSegment(carPos, pts[entryIndex], pts[(entryIndex + 1) % pts.Count]);

        int exitIndex = FindNearestSegment(destination, pts);
        Vector3 exitPoint = ProjectOntoSegment(destination, pts[exitIndex], pts[(exitIndex + 1) % pts.Count]);

        List<Vector3> pathForward = BuildDirectionalPath(pts, entryIndex, exitIndex, true, entryPoint, exitPoint);
        List<Vector3> pathBackward = BuildDirectionalPath(pts, entryIndex, exitIndex, false, entryPoint, exitPoint);

        float distF = CalculatePathDistance(pathForward);
        float distB = CalculatePathDistance(pathBackward);

        List<Vector3> bestPath = (distF <= distB) ? pathForward : pathBackward;

        bestPath.Insert(0, carPos);
        bestPath.Add(destination);

        return bestPath;
    }

    private int FindNearestSegment(Vector3 target, List<Vector3> pts)
    {
        int bestIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < pts.Count; i++)
        {
            Vector3 a = pts[i];
            Vector3 b = pts[(i + 1) % pts.Count];
            float dist = Vector3.Distance(target, ProjectOntoSegment(target, a, b));

            if (dist < minDistance)
            {
                minDistance = dist;
                bestIndex = i;
            }
        }
        return bestIndex;
    }

    private Vector3 ProjectOntoSegment(Vector3 pt, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        float t = Vector3.Dot(pt - a, ab) / Vector3.Dot(ab, ab);
        return a + Mathf.Clamp01(t) * ab; 
    }

    private List<Vector3> BuildDirectionalPath(List<Vector3> pts, int startSeg, int endSeg, bool forward, Vector3 entryPt, Vector3 exitPt)
    {
        List<Vector3> path = new List<Vector3>();
        path.Add(entryPt);

        if (startSeg == endSeg) 
        {
            path.Add(exitPt);
            return path;
        }

        int count = pts.Count;
        int currentIndex = forward ? (startSeg + 1) % count : startSeg;
        int targetIndex = forward ? endSeg : (endSeg + 1) % count;

        int failsafe = 0;
        while (failsafe < count + 2)
        {
            path.Add(pts[currentIndex]);
            if (currentIndex == targetIndex) break;
            
            currentIndex = forward ? (currentIndex + 1) % count : (currentIndex - 1 + count) % count;
            failsafe++;
        }

        path.Add(exitPt);
        return path;
    }

    private float CalculatePathDistance(List<Vector3> path)
    {
        float total = 0f;
        for (int i = 0; i < path.Count - 1; i++)
            total += Vector3.Distance(path[i], path[i + 1]);
        return total;
    }
}