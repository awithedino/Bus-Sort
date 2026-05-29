using UnityEngine;
using UnityEngine.SceneManagement; // We need this to load scenes!

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("InGame"); 
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game! 🚪💨");
        Application.Quit();
    }
}