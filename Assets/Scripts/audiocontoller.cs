using BloomJam.Audio;
using UnityEngine;
using YigitcanCaliskan.ServiceLocator;

public class audiocontoller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ServiceLocator.Get<IAudioService>().PlayBGM(BGMTrack.Gameplay);                                                                                                                                   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
