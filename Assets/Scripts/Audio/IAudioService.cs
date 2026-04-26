using UnityEngine;

namespace BloomJam.Audio
{
    public enum BGMTrack { MainMenu, Gameplay, Credits }

    public interface IAudioService
    {
        void PlaySFX(AudioClip clip, float volume = 1f);
        void PlayBGM(BGMTrack track);
        void StopBGM();
        void FadeBGM(float targetVolume, float duration);
    }
}