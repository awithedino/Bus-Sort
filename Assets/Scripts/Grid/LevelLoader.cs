using UnityEngine;

public class LevelLoader : MonoBehaviour 
{
    public GridManager gridManager;
    
    [Header("Level Data")]
    public LevelDataSO currentLevel;

    void Start() 
    {
        if (currentLevel != null) 
        {
            LoadLevelFromSO();
        }
    }

    public void LoadLevelFromSO() 
    {
        gridManager.InitializeGrid(currentLevel.gridWidth, currentLevel.gridHeight);
        foreach (CarSpawnData carData in currentLevel.carsInLevel) 
        {
            gridManager.SpawnVehicle(carData.carPrefab, carData.startX, carData.startY, carData.moveDirection);
        }

        // TODO: Feed the funnels
        // Dictionary<ColorType, int> passengerDeck = currentLevel.CalculateRequiredPassengers();
    }       
}