using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct FreeformCarData
{
    public Vehicle carPrefab;
    public Vector3 position;
    public Quaternion rotation; // Captures custom angles!
    public ColorType paintColor;
}

[CreateAssetMenu(fileName = "NewFreeformLevel", menuName = "Levels/Freeform Level")]
public class FreeformLevelSO : ScriptableObject
{
    public List<FreeformCarData> carsInLevel;
}