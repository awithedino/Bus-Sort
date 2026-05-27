using UnityEngine;
using UnityEngine.Splines;

public class PassengerGroup : MonoBehaviour
{
    [Header("Group Info")]
    public ColorType myColor;
    
    [Header("Visuals")]
    public Renderer[] passengerMeshes;
    
    [Header("Movement Settings")]
    public float speed = 0.4f;
    public float stopSpacing = 0.12f;
    
    [Header("Current State")]
    public SplineContainer currentTrack;
    public float currentT = 1f;
    public PassengerGroup leader;

    void Update()
    {
        if (currentTrack == null) return;

        bool isClosedLoop = currentTrack.Spline.Closed;
        if (leader != null)
        {
            float targetT = leader.currentT + stopSpacing;
            if (isClosedLoop && targetT > 1f) targetT -= 1f;

            float distanceToTarget = currentT - targetT;
            if (isClosedLoop && distanceToTarget < 0f) distanceToTarget += 1f; 
            if (distanceToTarget > 0.01f) 
            {
                currentT -= speed * Time.deltaTime;
                if (isClosedLoop && currentT < 0f) currentT += 1f;
                
                UpdateTransform();
            }
        }
        else 
        {
            if (isClosedLoop) 
            {
                currentT -= speed * Time.deltaTime;
                if (currentT < 0f) currentT += 1f;
                
                UpdateTransform();
            }
            else 
            {
                if (currentT > 0f) 
                {
                    currentT -= speed * Time.deltaTime;
                    currentT = Mathf.Max(currentT, 0f);
                    
                    UpdateTransform();
                }
            }
        }
    }
    
    public void SetupColor(ColorType newColor, Material colorMaterial)
    {
        myColor = newColor;
        
        foreach (Renderer mesh in passengerMeshes)
        {
            mesh.sharedMaterial = colorMaterial; 
        }
    }

    public void SetTrackAndLeader(SplineContainer track, float startT, PassengerGroup personInFront)
    {
        currentTrack = track;
        currentT = startT;
        leader = personInFront;
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        Vector3 worldPos = currentTrack.EvaluatePosition(currentT);
        transform.position = worldPos;
        
        Vector3 forwardDir = currentTrack.EvaluateTangent(currentT);
        if (forwardDir != Vector3.zero) 
        {
            transform.forward = forwardDir;
        }
    }
}