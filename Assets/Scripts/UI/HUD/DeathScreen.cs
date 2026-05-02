using BloomJam.Player;
using UnityEngine;
using YigitcanCaliskan.EventBus;
using YigitcanCaliskan.ServiceLocator;
using BloomJam;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] private GameObject deathScreenUI;

    private void OnEnable()  => EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
    private void OnDisable() => EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);

    private void OnPlayerDied(PlayerDiedEvent e)
    {
        deathScreenUI.SetActive(true);
        Time.timeScale = 0f;
        InputManager.Instance.SwitchToUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        InputManager.Instance.SwitchToGameplay();
        ServiceLocator.Get<ISceneService>().ReloadCurrent();
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        ServiceLocator.Get<ISceneService>().LoadMainMenu();
    }
}