using System;
using UnityEngine;
using YigitcanCaliskan.EventBus;
using BloomJam.Combat;
using BloomJam.Weapons;
using YigitcanCaliskan;
using YigitcanCaliskan.ServiceLocator;

namespace BloomJam.Enemies
{

    public class EnemyMainScript : MonoBehaviour, IDamageable
    {
        [Header("Identity")]
        [SerializeField] private EnemyType type;
        [SerializeField] private WeaponBehaviourKind weakTo;
        [SerializeField] private int pairingId;

        [Header("Stats")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float maxshotgundistance = 100f;

        public float CurrentHealth { get; private set; }
        public EnemyType Type => type;

        private bool _isDead;
        
        
        

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(in HitInfo hitinfo)
        { 
            if (_isDead) return;

            if ( hitinfo.SourceWeapon.BehaviourKind != weakTo )
            {
                return;
            }

            var new_damage=0f;
            
            if (maxshotgundistance > hitinfo.Distance)
            {
                new_damage  = hitinfo.SourceWeapon.BehaviourKind == WeaponBehaviourKind.Shotgun
                    ? hitinfo.BaseDamage * (1 - (hitinfo.Distance / maxshotgundistance))
                    : hitinfo.BaseDamage;
            }
           
           

            CurrentHealth -= new_damage;
            
            if (CurrentHealth <= 0f)
            {
                Die();
            }
        }

        private void Update()
        {
            if (ServiceLocator.Get<IInputService>().JumpPressed)
            {
                Die();
            }
        }

        private void Die()
        {
            EventBus.Publish(new EnemyDiedEvent
            {
                Type = type,
                PairingId = pairingId,
                Position = transform.position
            });

            Destroy(gameObject);
        }
    }
}
