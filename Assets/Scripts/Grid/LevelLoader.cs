using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{   
    public int gridWidth;
    public int gridHeight;
    public List<CarSaveData> cars;
}

[System.Serializable]
public class CarSaveData {
    public int carTypeIndex;
    public int startX;
    public int startY;
    public Direction moveDirection;
}

public class LevelLoader : MonoBehaviour {
    public GridManager gridManager;
    public List<Vehicle> carPrefabs;
    public TextAsset levelLayout;

    void Start() {
        if (levelLayout != null) {
            LoadLevelFromJson(levelLayout.text);
        }
    }

    public void LoadLevelFromJson(string jsonText) {
        LevelData data = JsonUtility.FromJson<LevelData>(jsonText);
        
        // Initiate the grid
        gridManager.InitializeGrid(data.gridWidth, data.gridHeight);
        
        foreach (var car in data.cars) {
            Vehicle prefab = carPrefabs[car.carTypeIndex];
            gridManager.SpawnVehicle(prefab, car.startX, car.startY, car.moveDirection);
        }
    }       
}