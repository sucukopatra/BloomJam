using System;
using BloomJam;
using BloomJam.Audio;
using UnityEngine;
using UnityEngine.Video;
using YigitcanCaliskan.ServiceLocator;

public class videoplayer_manager : MonoBehaviour
{     
  [SerializeField] private VideoPlayer videoPlayer;

  private void Start()
  {
    ServiceLocator.Get<IAudioService>().FadeBGM(0,0.3f);
    videoPlayer.loopPointReached += _ => SkipToGameplay();

  }

  private void Update()
  {
    if (Input.anyKeyDown)
    {
      SkipToGameplay();
    }
  }
  
  private void SkipToGameplay()
  {
    videoPlayer.loopPointReached -= _ => SkipToGameplay();
    ServiceLocator.Get<IAudioService>().PlayBGM(BGMTrack.Gameplay);
    ServiceLocator.Get<ISceneService>().LoadGameplay();
  }

  
}