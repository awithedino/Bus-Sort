using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class SplineQueueManager : MonoBehaviour
{
    public SplineContainer loopSpline;
    public List<PassengerGroup> loopQueue = new List<PassengerGroup>();
    
    public bool CanMergeAt(float mergeT, float safeDistance)
    {
        foreach (var group in loopQueue)
        {
            if (Mathf.Abs(group.currentT - mergeT) < safeDistance) return false;
        }
        return true;
    }
    
    public void MergeIntoLoop(PassengerGroup newGroup, float mergeT)
    {
        PassengerGroup newLeader = null;
        int insertIndex = 0;
        for (int i = 0; i < loopQueue.Count; i++)
        {
            if (loopQueue[i].currentT < mergeT)
            {
                newLeader = loopQueue[i];
                insertIndex = i + 1;
            }
        }

        if (insertIndex < loopQueue.Count)
        {
            loopQueue[insertIndex].leader = newGroup;
        }

        newGroup.SetTrackAndLeader(loopSpline, mergeT, newLeader);
        loopQueue.Insert(insertIndex, newGroup);
    }
    
    public void RemovePassengerFromLine(PassengerGroup boardedPassenger)
    {
        foreach (var group in loopQueue)
        {
            if (group.leader == boardedPassenger)
            {
                group.leader = boardedPassenger.leader; 
            }
        }
        loopQueue.Remove(boardedPassenger);
    }
    
    public void ResetQueue()
    {
        loopQueue.Clear();
    }
}