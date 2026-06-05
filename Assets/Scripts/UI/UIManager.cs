using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("Top Bar UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI winPanelCoinText;
    public TextMeshProUGUI winPanelLevelText;
    
    [Header("Settings Popup")]
    public GameObject settingsOverlay;
    public RectTransform settingsWindow;
    private Vector3 originalScale;

    void Start()
    {
        if (settingsOverlay != null)
        {
            if (settingsWindow != null) originalScale = settingsWindow.localScale;
            
            settingsOverlay.SetActive(false); 
        }
    }
    
    public void UpdateCoinText(int currentCoins)
    {
        if (winPanelCoinText != null) winPanelCoinText.text = currentCoins.ToString();
    }
    
    public void AnimateWinCoins(int oldTotal, int newTotal)
    {
        if (winPanelCoinText == null) return;
        
        int tempCoins = oldTotal;
        winPanelCoinText.text = tempCoins.ToString();
        DOTween.To(() => tempCoins, x => 
        {
            tempCoins = x;
            winPanelCoinText.text = tempCoins.ToString();
        }, newTotal, 1.5f).SetEase(Ease.OutQuad).SetUpdate(true);
    }
    
    public void UpdateLevelText(int levelIndex)
    {
        int displayLevel = levelIndex + 1; 
        levelText.text = "LEVEL " + displayLevel;
        if (winPanelLevelText != null) 
        {
            winPanelLevelText.text = "LEVEL " + displayLevel;
        }
    }

    public void OpenSettings()
    {
        settingsOverlay.SetActive(true);
        Time.timeScale = 0f; 
        
        settingsWindow.localScale = Vector3.zero;
        settingsWindow.DOScale(originalScale, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void CloseSettings()
    {
        settingsWindow.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() => 
        {
            settingsOverlay.SetActive(false);
            Time.timeScale = 1f; 
        });
    }
    
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        FindAnyObjectByType<LevelLoader>().RestartCurrentLevel();
        CloseSettings(); 
    }
    
    public void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }
}