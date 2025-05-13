using Gamecraft.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Gamecraft.Enemy
{
    public class EnemyBase : MonoBehaviour
    {
        [SerializeField] private Transform[] _patrolPoints;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _chaseSpeed = 5f;
        [SerializeField] private float _shootingDistance = 5f;
        [SerializeField] private float _detectionRange = 10f;
        [SerializeField] private float _closeDetectionRange = 3f;
        [SerializeField] private float _fieldOfViewAngle = 90f;
        [SerializeField] private float _waitUpdate = 0.5f;
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private float _waitTime = 2f;
        [SerializeField] private Animator _anmtr;
        [SerializeField] private GameObject _gun;
        [SerializeField] private AudioSource _shootSound;
        [SerializeField] private AudioSource _dieSound;
        [SerializeField] private AudioSource _meetSound;

        private bool isDead = false;
        private int currentPatrolIndex = 0;
        private Transform target;
        private Health targetHealth;
        private bool isChasing = false;
        private bool isWaiting = false;
        private float stopDistance = 3f;
        private float timerUpdate;

        private EnemyGun gun;
        private NavMeshAgent navMeshAgent;

        public UnityEvent OnMeetPlayer;
        public UnityEvent OnDie;

        private void Start()
        {
            Invoke("SendDataLater", 0.2f);
        }

        private void SendDataLater()
        {
            gun = GetComponent<EnemyGun>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.speed = _moveSpeed;
            GameManager.Instance.AudioEffects.Add(_shootSound);
            GameManager.Instance.AudioEffects.Add(_meetSound);
            GameManager.Instance.AudioEffects.Add(_dieSound);
        }

        public void TriggerAgresion()
        {
            if (target == null)
            {
                OnMeetPlayer.Invoke();
                StartCoroutine(PlayReactionAnimation());
            }
            target = GameManager.Instance.Player.transform;
        }

        private void Update()
        {
            if (isDead) return;

            timerUpdate += Time.deltaTime;
            if (timerUpdate > _waitUpdate)
            {
                if (target == null)
                {
                    _gun.SetActive(false);
                    if (!isWaiting && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + stopDistance)
                    {
                        if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                        {
                            Patrol();
                        }
                    }
                    SearchForTarget();
                }
                else
                {
                    _gun.SetActive(true);
                    ChaseTarget();
                }

                timerUpdate = 0;
            }
            _anmtr.SetBool("Run", navMeshAgent.isStopped ? false : true);
        }

        private void Patrol()
        {
            if (_patrolPoints.Length == 0) return;

            navMeshAgent.isStopped = false;
            Transform targetPoint = _patrolPoints[currentPatrolIndex];
            navMeshAgent.SetDestination(targetPoint.position);

            if (Vector3.Distance(transform.position, targetPoint.position) <= navMeshAgent.stoppingDistance + stopDistance)
            {
                StartCoroutine(WaitAtPoint());
            }
        }

        public void Death()
        {
            OnDie.Invoke();
            isDead = true;
        }

        private IEnumerator WaitAtPoint()
        {
            isWaiting = true;
            navMeshAgent.isStopped = true;
            yield return new WaitForSeconds(_waitTime);
            isWaiting = false;
            navMeshAgent.isStopped = false;

            currentPatrolIndex = GetRandomPatrolIndex();
            Patrol();
        }

        private int GetRandomPatrolIndex()
        {
            int newIndex;
            do
            {
                newIndex = Random.Range(0, _patrolPoints.Length);
            } while (newIndex == currentPatrolIndex && _patrolPoints.Length > 1);

            return newIndex;
        }

        private IEnumerator PlayReactionAnimation()
        {
            navMeshAgent.isStopped = true;
            _anmtr.SetTrigger("Meet");

            yield return new WaitForSeconds(2f);

            navMeshAgent.isStopped = false;
        }

        private void SearchForTarget()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRange);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider == transform.GetComponent<Collider>()) continue;

                Health health = hitCollider.GetComponent<Health>();
                EnemyBase ifOtherBot = hitCollider.GetComponent<EnemyBase>();

                if (health != null && ifOtherBot == null)
                {
                    Vector3 directionToTarget = (hitCollider.transform.position - transform.position).normalized;
                    float distanceToTarget = Vector3.Distance(transform.position, hitCollider.transform.position);

                    bool hasClearLineOfSight = CheckLineOfSight(hitCollider.transform);

                    if ((distanceToTarget <= _closeDetectionRange ||
                        (IsTargetInFieldOfView(directionToTarget) && hasClearLineOfSight)))
                    {
                        target = hitCollider.transform;
                        targetHealth = health;
                        isChasing = true;
                        StartCoroutine(PlayReactionAnimation());
                        OnMeetPlayer.Invoke();
                        navMeshAgent.speed = _chaseSpeed;
                        break;
                    }
                }
            }
        }

        private bool CheckLineOfSight(Transform targetTransform)
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 targetPos = targetTransform.position + Vector3.up * 0.5f;
            Vector3 direction = (targetPos - origin).normalized;
            float distance = Vector3.Distance(origin, targetPos);

            if (!Physics.Raycast(origin, direction, distance, _obstacleLayer))
            {
                return true;
            }

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, _obstacleLayer))
            {
                if (hit.transform == targetTransform)
                {
                    return true;
                }
            }

            Vector3[] rayOrigins = new Vector3[]
            {
        origin + Vector3.up * 0.2f,
        origin + Vector3.down * 0.2f,
        origin + transform.right * 0.2f,
        origin + -transform.right * 0.2f
            };

            foreach (var rayOrigin in rayOrigins)
            {
                Vector3 newDirection = (targetPos - rayOrigin).normalized;
                if (!Physics.Raycast(rayOrigin, newDirection, distance, _obstacleLayer))
                {
                    return true;
                }
            }

            return false;
        }

        private void ChaseTarget()
        {
            if (target == null) return;

            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget > _detectionRange * 1.5f)
            {
                target = null;
                targetHealth = null;
                isChasing = false;
                navMeshAgent.speed = _moveSpeed;
                navMeshAgent.isStopped = false;
                return;
            }

            if (distanceToTarget > _shootingDistance)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(target.position);
            }
            else
            {
                navMeshAgent.isStopped = true;
                Vector3 direction = (target.position - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f);
                }

                if (gun != null)
                {
                    gun.Shoot(_anmtr, _shootSound);
                }
            }
        }

        private bool IsTargetInFieldOfView(Vector3 directionToTarget)
        {
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            return angleToTarget < _fieldOfViewAngle * 0.5f;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _closeDetectionRange);

            Vector3 leftBound = Quaternion.Euler(0, -_fieldOfViewAngle * 0.5f, 0) * transform.forward * _detectionRange;
            Vector3 rightBound = Quaternion.Euler(0, _fieldOfViewAngle * 0.5f, 0) * transform.forward * _detectionRange;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + leftBound);
            Gizmos.DrawLine(transform.position, transform.position + rightBound);
        }
    }
}