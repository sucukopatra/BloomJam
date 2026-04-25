using UnityEngine;
using YigitcanCaliskan.EventBus;
using BloomJam.Combat;
public class Enemy_main_script : MonoBehaviour,IDamageable
{
    
    [Header("Identity")]
    [SerializeField] private EnemyType type;
    [SerializeField] private WeaponType weakTo;
    [SerializeField] private int pairingId;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public EnemyType Type => type;
    
    
    private void Awake()
    {
        CurrentHealth = maxHealth;
    }
    
    
    public void TakeDamage(in HitInfo hitinfo)
    {
       // if (hitinfo.SourceWeapon != weakTo) return;

        CurrentHealth -= hitinfo.BaseDamage;

        if (CurrentHealth <= 0f)
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
