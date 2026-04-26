using System.Collections;
using UnityEngine;
using BloomJam.Player;

namespace BloomJam.Enemies
{
    public class EnemyAI : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] private float detectionRange = 12f;
        [SerializeField] private float attackRange    = 1.5f;
        [SerializeField] private float attackCooldown  = 1f;
        [SerializeField] private float attackDamage    = 10f;
        [Tooltip("Animasyon başladıktan kaç saniye sonra hasar uygulanır")]
        [SerializeField] private float attackHitDelay  = 0.4f;

        [Header("Refs")]
        [SerializeField] private EnemyAnimator enemyAnimator;

        private IEnemyMovement _movement;
        private Transform      _player;
        private PlayerHealth   _playerHealth;
        private float          _nextAttackTime;

        private enum State { Idle, Chase, Attack }
        private State _state = State.Idle;

        private void Awake()
        {
            _movement = GetComponent<IEnemyMovement>();
        }

        private void Start()
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo == null) return;
            _player       = playerGo.transform;
            _playerHealth = playerGo.GetComponent<PlayerHealth>();
        }

        private void Update()
        {
            if (_player == null) return;

            var dist = Vector3.Distance(transform.position, _player.position);
            switch (_state)
            {
                case State.Idle:
                    if (dist <= detectionRange) EnterChase();
                    break;

                case State.Chase:
                    if (dist > detectionRange)   EnterIdle();
                    else if (dist <= attackRange) EnterAttack();
                    else
                    {
                        _movement.MoveTowards(_player.position);
                        enemyAnimator.FaceDirection(_player.position - transform.position);
                    }
                    break;

                case State.Attack:
                    if (dist > attackRange)                EnterChase();
                    else if (Time.time >= _nextAttackTime) DoAttack();
                    break;
            }
        }

        private void EnterIdle()
        {
            _state = State.Idle;
            _movement.Stop();
            enemyAnimator.PlayIdle();
        }

        private void EnterChase()
        {
            _state = State.Chase;
            enemyAnimator.PlayWalk();
        }

        private void EnterAttack()
        {
            _state = State.Attack;
            _movement.Stop();
            _nextAttackTime = 0f;
        }

        private void DoAttack()
        {
            enemyAnimator.PlayAttack();
            _nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(ApplyDamageAfterDelay());
        }

        private IEnumerator ApplyDamageAfterDelay()
        {
            yield return new WaitForSeconds(attackHitDelay);
            // Düşman öldüyse veya player uzaklaştıysa hasar verme
            if (!enabled) yield break;
            if (_player == null) yield break;
            if (Vector3.Distance(transform.position, _player.position) > attackRange * 1.5f) yield break;
            _playerHealth?.TakeDamage(attackDamage);
        }

        public void OnDeath()
        {
            enabled = false;
            StopAllCoroutines();
        }
    }
}