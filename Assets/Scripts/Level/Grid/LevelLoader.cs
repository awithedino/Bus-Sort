using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelLoader : MonoBehaviour 
{
    [Header("Dependencies")]
    public GridManager gridManager;
    public Transform vehicleParent; 
    
    [Header("Level Data")]
    public FreeformLevelSO currentLevel;
    private List<ColorType> passengerDeck = new List<ColorType>();
    
    [HideInInspector]
    public bool isRestarting = false;
    
    public void LoadFreeformLevel()
    {
        gridManager.ClearBoard();
        foreach (FreeformCarData data in currentLevel.carsInLevel)
        {
            Vehicle spawnedCar = Instantiate(data.carPrefab, data.position, data.rotation, vehicleParent);
            spawnedCar.ApplyColor(data.paintColor);
            gridManager.RegisterVehicle(spawnedCar);
        }
        BuildAndShuffleDeck();
    }

    private void BuildAndShuffleDeck()
    {
        Dictionary<ColorType, int> required = new Dictionary<ColorType, int>();
        
        foreach (FreeformCarData data in currentLevel.carsInLevel)
        {
            if (!required.ContainsKey(data.paintColor))
            {
                required[data.paintColor] = 0;
            }
            required[data.paintColor] += (data.carPrefab.capacity / 4); 
        }

        passengerDeck.Clear();
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

    public void RestartCurrentLevel()
    {
        isRestarting = true; 

        StuntPassenger[] activeStunts = FindObjectsByType<StuntPassenger>(FindObjectsSortMode.None);
        foreach (StuntPassenger stunt in activeStunts)
        {
            stunt.transform.DOKill();
            StuntPool.Instance.ReturnStunt(stunt);
        }
        
        Vehicle[] allCars = FindObjectsByType<Vehicle>(FindObjectsSortMode.None);
        foreach (Vehicle oldCar in allCars)
        {
            oldCar.transform.DOKill();
            Destroy(oldCar.gameObject);
        }
        
        PassengerGroup[] activeGroups = FindObjectsByType<PassengerGroup>(FindObjectsSortMode.None);
        foreach (PassengerGroup group in activeGroups)
        {
            group.transform.DOKill();
            group.leader = null; 
            PassengerPool.Instance.ReturnPassenger(group);
        }

        FindAnyObjectByType<SplineQueueManager>().ResetQueue();
        
        FunnelQueue[] allFunnels = FindObjectsByType<FunnelQueue>(FindObjectsSortMode.None);
        foreach (FunnelQueue funnel in allFunnels)
        {
            funnel.ResetQueue();
        }

        GameManager.Instance.LoadCurrentLevel();
        isRestarting = false; 
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