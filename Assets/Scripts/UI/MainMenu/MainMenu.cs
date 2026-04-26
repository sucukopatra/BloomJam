using UnityEngine;
using BloomJam;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneLoader.Instance.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame() => Application.Quit();
}