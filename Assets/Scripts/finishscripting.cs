using BloomJam.Player;
using UnityEngine;
using YigitcanCaliskan.EventBus;

public class finishscripting : MonoBehaviour
{
  
    void Start()
    {
        EventBus.Subscribe<PlayerDiedEvent>(debugfinish);
        
    }

    private void debugfinish(PlayerDiedEvent evt)
    {
        Debug.Log("player geberdi");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
