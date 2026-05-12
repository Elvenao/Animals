using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1); // tu escena de juego
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit"); // solo se ve en editor
    }
}
