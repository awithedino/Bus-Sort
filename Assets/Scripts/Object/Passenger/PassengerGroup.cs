using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;

public class PassengerGroup : MonoBehaviour
{
    [Header("Group Info")]
    public ColorType myColor;
    
    [Header("Visuals")]
    public Renderer[] passengerMeshes;
    private ParkingManager parkingManager;
    
    [Header("Movement Settings")]
    public float speed = 0.4f;
    public float stopSpacing = 0.12f;
    
    [Header("Current State")]
    public SplineContainer currentTrack;
    public float currentT = 1f;
    public PassengerGroup leader;
    
    [Header("Stunt Double")]
    public StuntPassenger stuntPrefab;
        
    private bool isBoarding; 

    void Awake()
    {
        parkingManager = FindAnyObjectByType<ParkingManager>();
    }
    
    void Update()
    {
        if (currentTrack == null) return;
        
        if (currentTrack.Spline.Closed && parkingManager != null) 
        {
            if (currentT >= 0.4f && currentT <= 0.6f)
            {
                Vehicle targetCar = parkingManager.FindMatchingCar(myColor);
                if (targetCar != null && !DOTween.IsTweening(targetCar.transform))
                {
                    AnimateBoarding(targetCar);
                    return;
                }
            }
        }
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
        isBoarding = false;
        UpdateTransform();
    }

    private void UpdateTransform()
    {
        if (currentTrack == null) return;
        
        Vector3 worldPos = currentTrack.EvaluatePosition(currentT);
        transform.position = worldPos;
        
        Vector3 forwardDir = currentTrack.EvaluateTangent(currentT);
        if (forwardDir != Vector3.zero) 
        {
            transform.forward = forwardDir;
        }
    }

    public void AnimateBoarding(Vehicle car)
    {
        car.currentCapacity += 4; 
        
        SplineQueueManager queueManager = FindAnyObjectByType<SplineQueueManager>();
        if (queueManager != null)
        {
            queueManager.RemovePassengerFromLine(this);
        }
        
        for (int i = 0; i < passengerMeshes.Length; i++)
        {
            StuntPassenger stunt = StuntPool.Instance.GetStunt(passengerMeshes[i].transform.position, passengerMeshes[i].transform.rotation);
            float delay = i * 0.1f;
            bool isLast = (i == passengerMeshes.Length - 1);
            
            Material currentMat = passengerMeshes[i].sharedMaterial;
            stunt.JumpIntoCar(car, currentMat, delay, isLast);
        }
        
        currentTrack = null; 
        leader = null;
        PassengerPool.Instance.ReturnPassenger(this);
    }
}