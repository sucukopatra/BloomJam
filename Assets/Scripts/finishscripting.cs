using BloomJam.Player;
using UnityEngine;
using YigitcanCaliskan.EventBus;

public class finishscripting : MonoBehaviour
{ 
    private void OnEnable()
    {
        EventBus.Subscribe<PlayerDiedEvent>(debugfinish);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerDiedEvent>(debugfinish);
    }


    private void debugfinish(PlayerDiedEvent evt)
    {
        Debug.Log("player geberdi");
    }
   
}
