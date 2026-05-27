using UnityEngine;
using UnityEngine.Splines;
using DG.Tweening;
using UnityEditor.Splines;

public class FunnelSpawner : MonoBehaviour
{
    [Header("References")] 
    public SplineQueueManager loopManager;
    public SplineContainer entrySpline;
    public PassengerGroup passengers;

    [Header("Settings")] 
    public int targetLoopSlot = 6;
    public float spawnInterval = 3f;

    private float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval) spawnTimer = 0f;

        if (loopManager.CanSpawnAtSlot(targetLoopSlot))
        {
            SpawnAndRunToLoop();
        }
        else
        {
            Debug.Log($"The funnel is blocked.");
        }
    }

    private void SpawnAndRunToLoop()
    {
        Vector3 spawnPos = entrySpline.EvaluatePosition(0f);
        PassengerGroup newGroup = Instantiate(passengers, spawnPos, Quaternion.identity);
        
        float progress = 0f;
        DOTween.To(() => progress, x => {
            progress = x;
            newGroup.transform.position = entrySpline.EvaluatePosition(progress);
            newGroup.transform.forward = entrySpline.EvaluateTangent(progress);
        }, 1f, 1f).SetEase(Ease.Linear).OnComplete(() => {
            loopManager.SpawnPassengerAtSlot(newGroup, targetLoopSlot);
        });
    }
}
