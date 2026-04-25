using UnityEngine;

namespace BloomJam.Enemies
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void Reset()
        {
            animator       = GetComponent<Animator>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void PlayIdle()      => animator.Play(EnemyAnimations.Idle);
        public void PlayWalk()      => animator.Play(EnemyAnimations.Walk);
        public void PlayAttack()    => animator.Play(EnemyAnimations.Attack);
        public void PlayHurt()      => animator.Play(EnemyAnimations.Hurt);
        public void PlayDieNormal() => animator.Play(EnemyAnimations.DieNormal);
        public void PlayDieHead()   => animator.Play(EnemyAnimations.DieHeadshot);

        public void FaceDirection(Vector3 worldDir)
        {
            if (Mathf.Abs(worldDir.x) > 0.01f)
                spriteRenderer.flipX = worldDir.x < 0f;
        }
    }
}