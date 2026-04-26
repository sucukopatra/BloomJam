using System.Collections;
using UnityEngine;
using YigitcanCaliskan;
using YigitcanCaliskan.EventBus;
using BloomJam.Enemies;
using BloomJam.Player;
using YigitcanCaliskan.ServiceLocator;

namespace BloomJam.Audio
{
    public class AudioManager : MonoBehaviour, IBootstrapService, IAudioService
    {
        public static AudioManager Instance { get; private set; }

        [Header("Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("BGM Klipleri")]
        [SerializeField] private AudioClip mainMenuBGM;
        [SerializeField] private AudioClip gameplayBGM;
        [SerializeField] private AudioClip creditsBGM;
        [SerializeField, Range(0f, 1f)] private float bgmVolume = 0.6f;

        [Header("SFX Klipleri")]
        [SerializeField] private AudioClip playerHurtClip;
        [SerializeField] private AudioClip playerDeathClip;
        [SerializeField] private AudioClip enemyDeathClip;

        private void Awake() => Instance = this;

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
            EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
            EventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
            EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
            EventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
        }

        public void Register() => ServiceLocator.Register<IAudioService>(this);

        // ── BGM ──────────────────────────────────────────────
        public void PlayBGM(BGMTrack track)
        {
            var clip = track switch
            {
                BGMTrack.MainMenu  => mainMenuBGM,
                BGMTrack.Gameplay  => gameplayBGM,
                BGMTrack.Credits   => creditsBGM,
                _                  => null
            };
            PlayClip(clip);
        }

        private void PlayClip(AudioClip clip)
        {
            if (clip == null || bgmSource.clip == clip) return;
            bgmSource.clip   = clip;
            bgmSource.volume = bgmVolume;
            bgmSource.loop   = true;
            bgmSource.Play();
        }

        public void StopBGM() => bgmSource.Stop();

        public void FadeBGM(float targetVolume, float duration)
        {
            StopAllCoroutines();
            StartCoroutine(FadeRoutine(targetVolume, duration));
        }

        private IEnumerator FadeRoutine(float target, float duration)
        {
            float start = bgmSource.volume;
            for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
            {
                bgmSource.volume = Mathf.Lerp(start, target, t / duration);
                yield return null;
            }
            bgmSource.volume = target;
            if (target <= 0f) bgmSource.Stop();
        }

        // ── SFX ──────────────────────────────────────────────
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip, volume);
        }

        // ── EventBus handlers ────────────────────────────────
        private void OnPlayerDamaged(PlayerDamagedEvent e) => PlaySFX(playerHurtClip);

        private void OnPlayerDied(PlayerDiedEvent e)
        {
            PlaySFX(playerDeathClip);
            FadeBGM(0f, 1.5f);
        }

        private void OnEnemyDied(EnemyDiedEvent e) => PlaySFX(enemyDeathClip);
    }
}