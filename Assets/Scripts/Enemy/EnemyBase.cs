using Gamecraft.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
        [SerializeField] private float _waitTime = 2f; // Время ожидания у точки

        private int currentPatrolIndex = 0;
        private Transform target;
        private Health targetHealth;
        private bool isChasing = false;
        private bool isWaiting = false;
        private float stopDistance = 3f; // Дистанция остановки перед точкой

        void Update()
        {
            if (target == null)
            {
                if (!isWaiting)
                {
                    Patrol();
                }
                SearchForTarget();
            }
            else
            {
                ChaseTarget();
            }
        }

        void Patrol()
        {
            if (_patrolPoints.Length == 0) return;

            Transform targetPoint = _patrolPoints[currentPatrolIndex];
            float distanceToPoint = Vector3.Distance(transform.position, targetPoint.position);

            if (distanceToPoint > stopDistance)
            {
                // Двигаемся к точке
                Vector3 direction = (targetPoint.position - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, _moveSpeed * Time.deltaTime);

                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }
            }
            else
            {
                // Останавливаемся и ждем
                if (!isWaiting)
                {
                    StartCoroutine(WaitAtPoint());
                }
            }
        }

        private IEnumerator WaitAtPoint()
        {
            isWaiting = true;
            yield return new WaitForSeconds(_waitTime); // Ждем указанное время
            isWaiting = false;

            // Выбираем случайную следующую точку
            currentPatrolIndex = GetRandomPatrolIndex();
        }

        int GetRandomPatrolIndex()
        {
            int newIndex;
            do
            {
                newIndex = Random.Range(0, _patrolPoints.Length);
            } while (newIndex == currentPatrolIndex); // Убедимся, что новая точка не совпадает с текущей

            return newIndex;
        }

        void SearchForTarget()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _detectionRange);
            foreach (var hitCollider in hitColliders)
            {
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
                            break;
                        }
                    }
                }
            }
        }

        void ChaseTarget()
        {
            if (target == null) return;

            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget > _detectionRange * 1.5f)
            {
                target = null;
                targetHealth = null;
                isChasing = false;
                return;
            }

            if (distanceToTarget > _shootingDistance)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, target.position, _chaseSpeed * Time.deltaTime);

                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }
            }
            else
            {
                Vector3 direction = (target.position - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }

                Debug.Log("Цель в зоне поражения!");
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

