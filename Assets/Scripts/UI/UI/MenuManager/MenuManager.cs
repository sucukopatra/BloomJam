using UnityEngine;
using System.Collections.Generic;
using BloomJam.Audio;
using YigitcanCaliskan.ServiceLocator;

public class MenuManager : MonoBehaviour
{
    private readonly Stack<GameObject> menuStack = new Stack<GameObject>();

    public GameObject startingMenu;

    void Start()
    {
        OpenMenu(startingMenu);
        ServiceLocator.Get<IAudioService>().PlayBGM(BGMTrack.MainMenu);
        InputManager.Instance.SwitchToUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        InputManager.Instance.OnCancel += OnCancel;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnCancel -= OnCancel;
    }

    void OnCancel() => GoBack();

    public void OpenMenu(GameObject menuToOpen)
    {
        if (menuToOpen == null) return;
        if (menuStack.Count > 0 && menuStack.Peek() == menuToOpen) return;

        if (menuStack.Count > 0)
            menuStack.Peek().SetActive(false);

        menuToOpen.SetActive(true);
        menuStack.Push(menuToOpen);
    }

    public void GoBack()
    {
        if (menuStack.Count <= 1) return;

        menuStack.Pop().SetActive(false);
        menuStack.Peek().SetActive(true);
    }
}
