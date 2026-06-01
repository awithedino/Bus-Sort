using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour 
{
    public GridManager gridManager;
    
    [Header("Level Data")]
    public LevelDataSO currentLevel;

    private List<ColorType> passengerDeck = new List<ColorType>();

    public void LoadLevelFromSO() 
    {
        gridManager.InitializeGrid(currentLevel.gridWidth, currentLevel.gridHeight);
        
        foreach (CarSpawnData carData in currentLevel.carsInLevel) 
        {
            gridManager.SpawnVehicle(carData.carPrefab, carData.startX, carData.startY, carData.moveDirection);
        }

        BuildAndShuffleDeck();
    }

    private void BuildAndShuffleDeck()
    {
        Dictionary<ColorType, int> required = currentLevel.CalculateRequiredPassengers();
        passengerDeck.Clear();

        // Get a list of the colors we still need to spawn
        List<ColorType> availableColors = new List<ColorType>(required.Keys);

        while (availableColors.Count > 0)
        {
            int randomColorIndex = Random.Range(0, availableColors.Count);
            ColorType chosenColor = availableColors[randomColorIndex];
            int batchSize = Random.Range(3, 6); 
            int amountToSpawn = Mathf.Min(batchSize, required[chosenColor]);
            for (int i = 0; i < amountToSpawn; i++)
            {
                passengerDeck.Add(chosenColor);
            }
            required[chosenColor] -= amountToSpawn;
            if (required[chosenColor] <= 0)
            {
                availableColors.RemoveAt(randomColorIndex);
            }
        }
    }
    
    public bool TryDrawPassenger(out ColorType drawnColor)
    {
        if (passengerDeck.Count > 0)
        {
            drawnColor = passengerDeck[0];
            passengerDeck.RemoveAt(0);
            return true;
        }
        drawnColor = default;
        return false;
    }
}