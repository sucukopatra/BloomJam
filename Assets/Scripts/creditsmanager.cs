using System;
using BloomJam;
using BloomJam.Audio;
using UnityEngine;
using UnityEngine.Video;
using YigitcanCaliskan.ServiceLocator;

public class creditsmanager : MonoBehaviour
{     
    [SerializeField] private VideoPlayer videoPlayer;

    private void Start()
    {
        ServiceLocator.Get<IAudioService>().FadeBGM(0,0.3f);
        videoPlayer.loopPointReached += _ => SkipToMainmenu();

    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
        //    SkipToMainmenu();
        }
    }
  
    private void SkipToMainmenu()
    {
        videoPlayer.loopPointReached -= _ => SkipToMainmenu();
        ServiceLocator.Get<IAudioService>().PlayBGM(BGMTrack.MainMenu);
        ServiceLocator.Get<ISceneService>().LoadMainMenu();
    }

  
}