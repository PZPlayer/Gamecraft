using Gamecraft.Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Gamecraft.Enemy
{
    public class BossAI : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _normalSpeed = 3.5f;
        [SerializeField] private float _chargeSpeed = 8f;
        [SerializeField] private float _playerDetectionRange = 10f;
        [SerializeField] private float _closeAttackRange = 2f;
        [SerializeField] private float _minChargeDistance = 5f;

        [Header("Attacks")]
        [SerializeField] private float _chargeDuration = 2f;  // Add Charge Duration
        [SerializeField] private float _chargeCooldown = 4f;
        [SerializeField] private float _chargeDamage = 15f;
        [SerializeField] private float _knockbackForce = 10f;
        [SerializeField] private float _knockbackUpForce = 5f;
        [SerializeField] private float _aoeRadius = 5f;
        [SerializeField] private float _aoeDamage = 25f;
        [SerializeField] private float _aoeCooldown = 10f;
        [SerializeField] private float _basicDamage = 10f;
        [SerializeField] private float _basicCooldown = 2f;

        public UnityEvent OnChargeAttack;
        public UnityEvent OnAoEAttack;
        public UnityEvent OnBasicAttack;

        private NavMeshAgent _agent;
        private Transform _player;
        [SerializeField] private Animator _animator;
        private Health _playerHealth;
        private Rigidbody _playerRb;

        private float _chargeTimer;
        private float _aoeTimer;
        private float _basicTimer;
        private Vector3 _chargeDirection;
        private bool _isCharging = false; // Add Charging flag
        private bool _isAoE = false;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = _normalSpeed;
            _agent.updateRotation = false; // Отключаем управление поворотом от NavMeshAgent
        }

        private void Start()
        {
            _player = GameManager.Instance.Player.transform;
            _playerHealth = _player.GetComponent<Health>();
            _playerRb = _player.GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_player == null) return;

            UpdateTimers();

            if (_isCharging) // If charging use ChargeMovement()
            {
                ChargeMovement();
            }
            else
            {
                HandleMovement();
                HandleAttacks();
            }
        }

        private void UpdateTimers()
        {
            if (_chargeTimer > 0) _chargeTimer -= Time.deltaTime;
            if (_aoeTimer > 0) _aoeTimer -= Time.deltaTime;
            if (_basicTimer > 0) _basicTimer -= Time.deltaTime;
        }

        private void HandleMovement()
        {
            if (_chargeTimer > 0 || !_isAoE)
            {
                _agent.speed = _normalSpeed;
                _agent.isStopped = false;
                _agent.SetDestination(_player.position);
                _animator.SetBool("IsWalking", _agent.destination != null);
                FaceTarget();
            }
        }

        private void FaceTarget()
        {
            Vector3 direction = (_player.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            }
        }

        private void HandleAttacks()
        {
            float distance = Vector3.Distance(transform.position, _player.position);

            if (_isAoE) return;

            if (_chargeTimer <= 0 && distance >= _minChargeDistance)
                StartCharge();
            else if (_aoeTimer <= 0 && distance <= _aoeRadius)
                StartAoEAttack();
            else if (_basicTimer <= 0 && distance <= _closeAttackRange)
                StartBasicAttack();
        }

        private void StartCharge()
        {
            _chargeTimer = _chargeCooldown + _chargeDuration;
            _chargeDirection = transform.forward;
            _agent.isStopped = true;
            _agent.speed = _chargeSpeed;
            _isCharging = true;  // Start Charging
            _animator.SetTrigger("Charge");
            OnChargeAttack.Invoke();
        }
        // Charge attack
        private void ChargeMovement()
        {
            if (_chargeTimer > 0)
            {
                transform.position += _chargeDirection * _chargeSpeed * Time.deltaTime;
            }
            else
            {
                EndAttack();
            }
        }

        private void StartAoEAttack()
        {
            _aoeTimer = _aoeCooldown;
            _isAoE = true;
            _agent.isStopped = true;
            _animator.SetTrigger("AoE");
            Invoke("ApplyAoEDamage", 3f);
            Invoke("EndAttack", 3f);
        }

        private void StartBasicAttack()
        {
            _basicTimer = _basicCooldown;
            _agent.isStopped = true;
            _animator.SetTrigger("Attack");
            OnBasicAttack.Invoke();
            Invoke("ApplyBasicDamage", 0.5f);
            Invoke("EndAttack", 1f);
        }

        private void ApplyAoEDamage()
        {
            OnAoEAttack.Invoke();
            Collider[] hits = Physics.OverlapSphere(transform.position, _aoeRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                    hit.GetComponent<Health>()?.DamageBody((int)_aoeDamage);
            }
        }

        private void ApplyBasicDamage()
        {
            if (Vector3.Distance(transform.position, _player.position) <= _closeAttackRange)
                _playerHealth.DamageBody((int)_basicDamage);
        }

        private void EndAttack()
        {
            _agent.isStopped = false;
            _isAoE = false;
            _isCharging = false; // End charging
            _animator.SetTrigger("Reset");
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isCharging) // During Charge
            {
                print(collision.transform.tag);
                if (collision.gameObject.CompareTag("Player"))
                {
                    _playerHealth.DamageBody((int)_chargeDamage);
                    Vector3 knockbackDir = (collision.transform.position - transform.position).normalized;
                    knockbackDir.y = _knockbackUpForce / _knockbackForce;
                    _playerRb.AddForce(knockbackDir * _knockbackForce, ForceMode.Impulse);
                    EndAttack();
                }
                else if (collision.gameObject.CompareTag("Wall"))
                {
                    _animator.SetTrigger("Stun");
                    EndAttack();
                }
            }
        }
    }
}