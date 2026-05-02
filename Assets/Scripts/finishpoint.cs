using System;
using BloomJam;
using BloomJam.Audio;
using UnityEngine;
using YigitcanCaliskan.ServiceLocator;

public class finishpoint : MonoBehaviour
{
  private void OnTriggerEnter(Collider other)
  {
    if (other.tag == "Player")
    {
      _finish();
    }
  }

  private void _finish()
  {
    ServiceLocator.Get<IAudioService>().FadeBGM(0,0.3f);
    ServiceLocator.Get<ISceneService>().LoadCredit();
  }
  
}