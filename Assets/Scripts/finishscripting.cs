using System;
using BloomJam;
using BloomJam.Player;
using UnityEngine;
using YigitcanCaliskan;
using YigitcanCaliskan.EventBus;
using YigitcanCaliskan.ServiceLocator;

public class finishscripting : MonoBehaviour
{ 
    private void OnEnable()
    {
        ServiceLocator.Get<IInputService>().OnReload += downlo;
        EventBus.Subscribe<PlayerDiedEvent>(debugfinish);
    }

    private void OnDisable()
    {        ServiceLocator.Get<IInputService>().OnReload -= downlo;

        EventBus.Unsubscribe<PlayerDiedEvent>(debugfinish);
    }

    public void downlo()
    {
        ServiceLocator.Get<ISceneService>().LoadMainMenu();
    }
  

    private void debugfinish(PlayerDiedEvent evt)
    {
        Debug.Log("player geberdi");
    }
   
}
