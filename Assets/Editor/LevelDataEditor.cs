using UnityEngine;
using UnityEditor; // Required for making custom inspectors!

[CustomEditor(typeof(LevelDataSO))]
public class LevelDataEditor : Editor
{
    private int selectedCarIndex = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelDataSO level = (LevelDataSO)target;

        if (level.carsInLevel == null || level.carsInLevel.Count == 0)
        {
            EditorGUILayout.HelpBox("Add at least one car to the list above to start painting on the grid!", MessageType.Info);
            return;
        }

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Visual Grid Painter", EditorStyles.boldLabel);
        selectedCarIndex = EditorGUILayout.IntSlider("Selecting Car:", selectedCarIndex, 0, level.carsInLevel.Count - 1);

        CarSpawnData activeCar = level.carsInLevel[selectedCarIndex];
        string carName = activeCar.carPrefab != null ? activeCar.carPrefab.name : "Empty Prefab";
        EditorGUILayout.LabelField($"Currently Placing: {carName}", EditorStyles.miniLabel);

        GUILayout.Space(10);
        for (int y = level.gridHeight - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < level.gridWidth; x++)
            {
                string cellLabel = "-";
                Color defaultColor = GUI.backgroundColor;
                for (int i = 0; i < level.carsInLevel.Count; i++)
                {
                    if (level.carsInLevel[i].startX == x && level.carsInLevel[i].startY == y)
                    {
                        cellLabel = i.ToString();
                        if (i == selectedCarIndex) GUI.backgroundColor = Color.green;
                        else GUI.backgroundColor = Color.gray;
                        break;
                    }
                }
                
                if (GUILayout.Button(cellLabel, GUILayout.Width(40), GUILayout.Height(40)))
                {
                    Undo.RecordObject(level, "Move Car On Grid");
                    activeCar.startX = x;
                    activeCar.startY = y;
                    EditorUtility.SetDirty(level);
                }
                
                GUI.backgroundColor = defaultColor;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}