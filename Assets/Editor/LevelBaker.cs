using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LevelBaker : Editor
{
    [MenuItem("Tools/Bake Scene to Freeform Level")]
    public static void BakeLevel()
    {
        Vehicle[] allCars = FindObjectsByType<Vehicle>(FindObjectsSortMode.None);
        if (allCars.Length == 0)
        {
            Debug.LogWarning("No cars found in the scene to save!");
            return;
        }
        
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Freeform Level",
            "Level_01", // Default name
            "asset",
            "Choose a name and location for your level"
        );
        
        if (string.IsNullOrEmpty(path)) return; 
        
        FreeformLevelSO newLevel = ScriptableObject.CreateInstance<FreeformLevelSO>();
        newLevel.carsInLevel = new List<FreeformCarData>();
        
        foreach (Vehicle car in allCars)
        {
            FreeformCarData data = new FreeformCarData();
            
            data.carPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(car);
            data.position = car.transform.position;
            data.rotation = car.transform.rotation;
            data.paintColor = car.carColor;

            newLevel.carsInLevel.Add(data);
        }
        AssetDatabase.CreateAsset(newLevel, path);
        // Set the file dirty
        EditorUtility.SetDirty(newLevel);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Success! Baked {allCars.Length} cars into {path} 🚗✨");
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newLevel;
    }
}