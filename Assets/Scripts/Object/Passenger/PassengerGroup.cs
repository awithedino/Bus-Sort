using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;

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
        
    private bool isBoarding = false; 

    void Update()
    {
        // If we are already boarding, don't execute any line-following logic!
        if (currentTrack == null || isBoarding) return;
        
        float previousT = currentT; 

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

        // The Pure-Math Scanner!
        if (currentTrack != null && currentTrack.Spline.Closed) 
        {
            float checkPoint = 0.5f; 
            
            if (previousT > checkPoint && currentT <= checkPoint)
            {
                Vehicle targetCar = FindAnyObjectByType<ParkingManager>().FindMatchingCar(myColor);
                
                if (targetCar != null)
                {
                    AnimateBoarding(targetCar); 
                    return; 
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
        isBoarding = true;
        car.currentCapacity += 4; 
        
        SplineQueueManager queueManager = FindAnyObjectByType<SplineQueueManager>();
        if (queueManager != null)
        {
            queueManager.RemovePassengerFromLine(this);
        }
        
        currentTrack = null; 
        leader = null;
        transform.DOJump(car.transform.position, 1.5f, 1, 0.5f).SetEase(Ease.InOutQuad).OnComplete(() => 
        {
            if (car.audioSource != null && car.hopInSound != null)
            {
                car.audioSource.pitch = Random.Range(0.9f, 1.1f);
                car.audioSource.PlayOneShot(car.hopInSound);
            }
            car.boardedCapacity += 4;
            car.UpdateCapacityUI();
            
            if (car.boardedCapacity >= car.capacity)
            {
                car.SetTextVisible(false);
                FindAnyObjectByType<ParkingManager>().DispatchCar(car);
            }
            
            PassengerPool.Instance.ReturnPassenger(this);
        });
    }
}