using System.Collections.Generic;
using UnityEngine;

public class StuntPool : MonoBehaviour
{
    public static StuntPool Instance { get; private set; }
    
    public StuntPassenger stuntPrefab;
    public int initialPoolSize = 20;

    private Queue<StuntPassenger> pool = new Queue<StuntPassenger>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewStunt();
        }
    }

    private void CreateNewStunt()
    {
        StuntPassenger newStunt = Instantiate(stuntPrefab, transform);
        newStunt.gameObject.SetActive(false);
        pool.Enqueue(newStunt);
    }

    public StuntPassenger GetStunt(Vector3 position, Quaternion rotation)
    {
        if (pool.Count == 0) CreateNewStunt();

        StuntPassenger stunt = pool.Dequeue();
        stunt.transform.position = position;
        stunt.transform.rotation = rotation;
        stunt.gameObject.SetActive(true);
        return stunt;
    }

    public void ReturnStunt(StuntPassenger stunt)
    {
        stunt.gameObject.SetActive(false);
        pool.Enqueue(stunt);
    }
}