using BloomJam.Combat;
using UnityEngine;

public interface IDamageable 
{
   public void TakeDamage(in HitInfo hitInfo);
}
