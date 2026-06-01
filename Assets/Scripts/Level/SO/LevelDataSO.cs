using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level_", menuName = "Puzzle/New Level")]
public class LevelDataSO : ScriptableObject 
{
    [Header("Grid Setup")]
    public int gridWidth = 6;
    public int gridHeight = 6;
    
    [Header("Vehicles")]
    public List<CarSpawnData> carsInLevel;
    
    public Dictionary<ColorType, int> CalculateRequiredPassengers()
    {
        Dictionary<ColorType, int> colorCounts = new Dictionary<ColorType, int>();

        foreach (var carData in carsInLevel)
        {
            // Assuming each car holds 16, and each group is 4 people
            int groupsNeeded = carData.carPrefab.capacity / 4; 

            if (colorCounts.ContainsKey(carData.carPrefab.carColor)) {
                colorCounts[carData.carPrefab.carColor] += groupsNeeded;
            } else {
                colorCounts.Add(carData.carPrefab.carColor, groupsNeeded);
            }
        }
        return colorCounts;
    }
}

[System.Serializable]
public class CarSpawnData 
{
    public Vehicle carPrefab; // Drag the actual prefab here now!
    public int startX;
    public int startY;
    public Direction moveDirection;
}