using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class FunnelQueue : MonoBehaviour
{
    [Header("References")]
    public SplineQueueManager mainLoop;
    public SplineContainer funnelSpline;
    public PassengerGroup passengerPrefab;
    public Transform passengerContainer;

    [Header("Settings")]
    public float mergeTOnMainLoop = 0.8f;
    public int maxCapacity = 4;
    public float spawnInterval = 3f;
    
    [Header("Color Materials (0 = Red, 1 = Blue, 2 = Green, 3 = Orange)")]
    public Material[] colorMaterials;

    private List<PassengerGroup> funnelQueue = new List<PassengerGroup>();
    private float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval && funnelQueue.Count < maxCapacity)
        {
            spawnTimer = 0f;
            SpawnPassenger();
        }
        
        if (funnelQueue.Count > 0)
        {
            PassengerGroup frontPerson = funnelQueue[0];
            if (frontPerson.currentT <= 0.05f && mainLoop.CanMergeAt(mergeTOnMainLoop, frontPerson.stopSpacing))
            {
                funnelQueue.RemoveAt(0);
                if (funnelQueue.Count > 0) funnelQueue[0].leader = null;
                mainLoop.MergeIntoLoop(frontPerson, mergeTOnMainLoop);
            }
        }
    }

    void SpawnPassenger()
    {
        Vector3 spawnPos = funnelSpline.EvaluatePosition(1f); 
        PassengerGroup newGroup = Instantiate(passengerPrefab, spawnPos, Quaternion.identity, passengerContainer);

        // Random color
        int randomColorIndex = Random.Range(0, colorMaterials.Length);
        ColorType chosenColor = (ColorType)randomColorIndex;
        Material chosenMat = colorMaterials[randomColorIndex];

        // Paint
        newGroup.SetupColor(chosenColor, chosenMat);
        PassengerGroup leader = funnelQueue.Count > 0 ? funnelQueue[funnelQueue.Count - 1] : null;
        newGroup.SetTrackAndLeader(funnelSpline, 1f, leader);
        
        funnelQueue.Add(newGroup);
    }
}