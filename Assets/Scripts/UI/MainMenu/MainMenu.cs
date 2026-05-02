using BloomJam;
using UnityEngine;
using UnityEngine.SceneManagement;
using YigitcanCaliskan;
using YigitcanCaliskan.ServiceLocator;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        ServiceLocator.Get<ISceneService>().LoadScene(2);

    }
    public void QuitGame(){
        Application.Quit();
    }
}
