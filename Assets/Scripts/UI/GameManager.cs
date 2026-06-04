using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LevelLoader levelLoader;
    public GameObject winPanel;
    public GameObject finalWinPanel;
    public CanvasGroup loadingScreen;
    
    private int totalCarsInLevel;
    private int carsEscaped;
    
    [Header("Levels")]
    public List<FreeformLevelSO> allLevels; 
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
        StartCoroutine(LoadingSequence());
    }
    
    private System.Collections.IEnumerator LoadingSequence()
    {
        if (loadingScreen != null)
        {
            loadingScreen.gameObject.SetActive(true);
            loadingScreen.alpha = 0f;
            loadingScreen.DOFade(1f, 0.4f).SetUpdate(true);
        }

        yield return new WaitForSecondsRealtime(1f);
        
        CancelInvoke("LevelComplete"); 
        FindAnyObjectByType<UIManager>().UpdateLevelText(currentLevelIndex);
        
        carsEscaped = 0;
        totalCarsInLevel = allLevels[currentLevelIndex].carsInLevel.Count;
        
        levelLoader.currentLevel = allLevels[currentLevelIndex];
        levelLoader.LoadFreeformLevel(); 

        if (loadingScreen != null)
        {
            Tween fadeOut = loadingScreen.DOFade(0f, 0.4f).SetUpdate(true);
            yield return fadeOut.WaitForCompletion();
            loadingScreen.gameObject.SetActive(false);
        }
    }
    
    public void CarEscaped()
    {
        if (FindAnyObjectByType<LevelLoader>().isRestarting) return;
        carsEscaped++;
        if (carsEscaped >= totalCarsInLevel)
        {
            Invoke("LevelComplete", 1.5f); 
        }
    }
    
    public void LevelComplete()
    {
        if (FindAnyObjectByType<LevelLoader>().isRestarting) return;
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