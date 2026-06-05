using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LevelLoader levelLoader;
    public GameObject winPanel;
    public GameObject finalWinPanel;
    public GameObject losePanel;
    public CanvasGroup loadingScreen;
    
    [Header("Currency")]
    public int totalCoins = 300;
    
    private int totalCarsInLevel;
    private int carsEscaped;
    
    [Header("Levels")]
    public List<FreeformLevelSO> allLevels; 
    private int currentLevelIndex = 0;
    
    [HideInInspector] public bool isGameOver = false;

    void Awake()
    {
        Instance = this;
        currentLevelIndex = PlayerPrefs.GetInt("SavedLevel", 0);
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
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
        UIAudioManager.Instance.ChangeMusic(UIAudioManager.Instance.gameplayMusic, true); 
        
        if (losePanel != null) losePanel.SetActive(false);
    }
    
    private IEnumerator LoadingSequence()
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
        
        isGameOver = false;
        PlayerInput input = FindAnyObjectByType<PlayerInput>();
        if (input != null) input.enabled = true;

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
        if (isGameOver || FindAnyObjectByType<LevelLoader>().isRestarting) return;
        isGameOver = true; 
        int oldCoins = totalCoins;
        totalCoins += 20;
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();
        if (currentLevelIndex >= allLevels.Count - 1)
        {
            ShowPanelWithPop(finalWinPanel); 
            UIAudioManager.Instance.ChangeMusic(UIAudioManager.Instance.finalWinMusic, false);
        }
        else
        {
            ShowPanelWithPop(winPanel);
            UIAudioManager.Instance.ChangeMusic(UIAudioManager.Instance.winMusic, false);
        }
        FindAnyObjectByType<UIManager>().AnimateWinCoins(oldCoins, totalCoins);
    }
    
    public void GameOver()
    {
        if (isGameOver || FindAnyObjectByType<LevelLoader>().isRestarting) return;
        isGameOver = true; 
        ShowPanelWithPop(losePanel);
        if (UIAudioManager.Instance.loseMusic != null) 
        {
            UIAudioManager.Instance.ChangeMusic(UIAudioManager.Instance.loseMusic, false);
        }
        PlayerInput input = FindAnyObjectByType<PlayerInput>();
        if (input != null) input.enabled = false;
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0); 
    }
    
    public void LoadNextLevel()
    {
        currentLevelIndex++;
        
        // If we beat the final level, loop back to Level 1! 🔄
        if (currentLevelIndex >= allLevels.Count)
        {
            currentLevelIndex = 0; 
        }
        
        PlayerPrefs.SetInt("SavedLevel", currentLevelIndex);
        PlayerPrefs.Save(); 

        levelLoader.gridManager.ClearBoard(); 
        
        if (winPanel != null) winPanel.SetActive(false);
        if (finalWinPanel != null) finalWinPanel.SetActive(false);
        
        LoadCurrentLevel();
    }
    
    private void ShowPanelWithPop(GameObject panel)
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
    }
}