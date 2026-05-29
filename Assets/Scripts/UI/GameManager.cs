using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LevelLoader levelLoader;
    public GameObject winPanel;
    public GameObject finalWinPanel;
    
    private int totalCarsInLevel;
    private int carsEscaped;
    
    [Header("Levels")]
    public List<LevelDataSO> allLevels;
    private int currentLevelIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        winPanel.SetActive(false);
        finalWinPanel.SetActive(false);
        LoadCurrentLevel();
    }

    public void LoadCurrentLevel()
    {
        carsEscaped = 0;
        totalCarsInLevel = allLevels[currentLevelIndex].carsInLevel.Count;
        
        levelLoader.currentLevel = allLevels[currentLevelIndex];
        levelLoader.LoadLevelFromSO();
    }
    
    public void CarEscaped()
    {
        carsEscaped++;
        
        if (carsEscaped >= totalCarsInLevel)
        {
            Invoke("LevelComplete", 1.5f); 
        }
    }
    
    public void LevelComplete()
    {
        if (currentLevelIndex >= allLevels.Count - 1)
        {
            finalWinPanel.SetActive(true);
        }
        else
        {
            winPanel.SetActive(true);
        }
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0); 
    }
    
    public void LoadNextLevel()
    {
        currentLevelIndex++;
        
        if (currentLevelIndex >= allLevels.Count)
        {
            currentLevelIndex = 0;
            Debug.Log("All levels complete! 🏆");
        }
        levelLoader.gridManager.ClearBoard(); 
        winPanel.SetActive(false);
        LoadCurrentLevel();
    }
}