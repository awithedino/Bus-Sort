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
        
        string path = "Assets/NewFreeformLevel.asset";
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        
        AssetDatabase.CreateAsset(newLevel, path);
        AssetDatabase.SaveAssets();

        Debug.Log($"Success! Baked {allCars.Length} cars into {path} 🚗✨");
        
        // 5. Highlight the new file for you
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newLevel;
    }
}