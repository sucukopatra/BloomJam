using System.Collections;
using UnityEngine;
using YigitcanCaliskan;
using YigitcanCaliskan.EventBus;
using BloomJam.Enemies;
using BloomJam.Player;
using YigitcanCaliskan.ServiceLocator;

namespace BloomJam.Audio
{
    public class AudioManager : MonoBehaviour, IBootstrapService,IAudioService
    {
        public static AudioManager Instance { get; private set; }

        [Header("Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("SFX Klipleri")]
        [SerializeField] private AudioClip playerHurtClip;
        [SerializeField] private AudioClip playerDeathClip;
        [SerializeField] private AudioClip enemyDeathClip;

        [Header("BGM")]
        [SerializeField] private AudioClip defaultBGM;
        [SerializeField, Range(0f, 1f)] private float bgmVolume = 0.6f;

        private void Awake() => Instance = this;

        private void Start()
        {
            if (defaultBGM != null) PlayBGM(defaultBGM);
        }

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

        public void Register()
        {
            ServiceLocator.Register<IAudioService>(this);

        }

        // ── SFX ──────────────────────────────────────────────
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip, volume);
        }

        // ── BGM ──────────────────────────────────────────────
        public void PlayBGM(AudioClip clip)
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