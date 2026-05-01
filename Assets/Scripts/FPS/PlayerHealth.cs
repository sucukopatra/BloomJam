using BloomJam.Audio;
using UnityEngine;
using YigitcanCaliskan.EventBus;
using YigitcanCaliskan.ServiceLocator;

namespace BloomJam.Player
{
    public struct PlayerDamagedEvent : IEvent
    {
        public float CurrentHealth;
        public float MaxHealth;

        public PlayerDamagedEvent(float current, float max)
        {
            CurrentHealth = current;
            MaxHealth     = max;
        }
    }

    public struct PlayerDiedEvent : IEvent { }

    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;

        public float MaxHealth     => maxHealth;
        public float CurrentHealth { get; private set; }
        public bool  IsAlive       => CurrentHealth > 0f;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public AudioClip clip;
        public void TakeDamage(float amount)
        {
            if (!IsAlive) return;
            Debug.Log("çalışıyor");
            CurrentHealth -= amount;
            ServiceLocator.Get<IAudioService>().PlaySFX(clip);
           // CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            EventBus.Publish(new PlayerDamagedEvent(CurrentHealth, maxHealth));
            if (CurrentHealth <= 0f)
                EventBus.Publish(new PlayerDiedEvent());
        }

       
    }
}