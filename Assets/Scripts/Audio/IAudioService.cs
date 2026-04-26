using UnityEngine;

namespace BloomJam.Audio
{
    public interface IAudioService
    {
        void PlaySFX(AudioClip clip, float volume = 1f);
        void PlayBGM(AudioClip clip);
        void StopBGM();
        void FadeBGM(float targetVolume, float duration);
    }
}