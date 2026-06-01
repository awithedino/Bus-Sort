using System.Collections.Generic;
using UnityEngine;

public class PerimeterTrack : MonoBehaviour
{
    [Tooltip("Drag your 8 corner points here IN ORDER (like a clock face)")]
    public List<Transform> waypoints;

    public List<Vector3> GetShortestPath(Vector3 carPos, Vector3 destination)
    {
        if (waypoints == null || waypoints.Count < 3) return null;

        List<Vector3> pts = new List<Vector3>();
        foreach (var wp in waypoints) pts.Add(wp.position);

        // 1. Find exactly where the car merges onto the track
        int entryIndex = FindNearestSegment(carPos, pts);
        Vector3 entryPoint = ProjectOntoSegment(carPos, pts[entryIndex], pts[(entryIndex + 1) % pts.Count]);

        // 2. Find exactly where the destination branches off the track
        int exitIndex = FindNearestSegment(destination, pts);
        Vector3 exitPoint = ProjectOntoSegment(destination, pts[exitIndex], pts[(exitIndex + 1) % pts.Count]);

        // 3. Build both clockwise and counter-clockwise routes
        List<Vector3> pathForward = BuildDirectionalPath(pts, entryIndex, exitIndex, true, entryPoint, exitPoint);
        List<Vector3> pathBackward = BuildDirectionalPath(pts, entryIndex, exitIndex, false, entryPoint, exitPoint);

        // 4. Measure them and pick the shortest route!
        float distF = CalculatePathDistance(pathForward);
        float distB = CalculatePathDistance(pathBackward);

        List<Vector3> bestPath = (distF <= distB) ? pathForward : pathBackward;

        // 5. Add the start and end to complete the journey
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
        return a + Mathf.Clamp01(t) * ab; // Math magic to snap point to line!
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