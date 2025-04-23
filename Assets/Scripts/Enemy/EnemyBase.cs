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
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private float _waitTime = 2f;
        [SerializeField] private Animator _anmtr;
        [SerializeField] private GameObject _gun;

        private bool isReactingToTarget = false;
        private int currentPatrolIndex = 0;
        private Transform target;
        private Health targetHealth;
        private bool isChasing = false;
        private bool isWaiting = false;
        private float stopDistance = 3f;

        private EnemyGun gun;
        private NavMeshAgent navMeshAgent;

        public UnityEvent OnMeetPlayer;

        private void Start()
        {
            gun = GetComponent<EnemyGun>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.speed = _moveSpeed;
        }

        private void Update()
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
                if (health != null)
                {
                    Vector3 directionToTarget = (hitCollider.transform.position - transform.position).normalized;
                    float distanceToTarget = Vector3.Distance(transform.position, hitCollider.transform.position);

                    if (distanceToTarget <= _closeDetectionRange || IsTargetInFieldOfView(directionToTarget))
                    {
                        if (!Physics.Linecast(transform.position, hitCollider.transform.position, _obstacleLayer))
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
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }

                if (gun != null)
                {
                    gun.Shoot(_anmtr);
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