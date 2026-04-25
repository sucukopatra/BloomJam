using UnityEngine;
using YigitcanCaliskan.EventBus;

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

        public void TakeDamage(float amount)
        {
            if (!IsAlive) return;
            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            EventBus.Publish(new PlayerDamagedEvent(CurrentHealth, maxHealth));
            if (CurrentHealth <= 0f)
                EventBus.Publish(new PlayerDiedEvent());
        }

        public void Heal(float amount)
        {
            if (!IsAlive) return;
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            EventBus.Publish(new PlayerDamagedEvent(CurrentHealth, maxHealth));
        }
    }
}