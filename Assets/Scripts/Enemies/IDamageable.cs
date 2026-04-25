namespace BloomJam.Combat
{
    public interface IDamageable
    {
        void TakeDamage(in HitInfo hitInfo);
    }
}
