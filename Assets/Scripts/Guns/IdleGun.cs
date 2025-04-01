using Gamecraft.Player;
using UnityEngine;

public interface IUsable
{
    public void Use();
}

namespace Gamecraft.Guns
{
    public class IdleGun : MonoBehaviour, IUsable
    {
        [SerializeField] protected Transform _shootPoint;
        [SerializeField] protected int _maxAmmo;
        [SerializeField] protected float _fireRate;
        [SerializeField] protected bool _ifAuto;
        [SerializeField] protected float _damage;
        [SerializeField] protected float _range;
        [SerializeField] protected LayerMask _layerMask;

        [SerializeField] private Animator _animator;

        [SerializeField] private Material lineMaterial;

        [SerializeField] private float lineDistance = 10f;
        [SerializeField] private float lineDuration = 0.2f;
        [SerializeField] private float lineWidth = 0.05f;

        private Vector3 cameraForward;

        private LineRenderer lineRenderer;

        private bool isLineActive = false;

        private float lineTimer = 0f;
        private float coolDown;

        public bool IsAiming;

        private void Start()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }

        public void Use()
        {
            if (coolDown < _fireRate) return;
            coolDown = 0;
            _animator.SetTrigger("Shoot");
            Invoke("Shoot", 0.2f);
        }

        private void Update()
        {
            IsAiming = GameManager.Instance.ifAiming;

            coolDown += Time.deltaTime;

            DrawLine();
        }

        private void DrawLine()
        {
            Vector3 cameraPosition = Camera.main.transform.position;
            cameraForward = Camera.main.transform.forward;

            Debug.DrawLine(cameraPosition, cameraPosition + cameraForward * lineDistance, Color.red);

            if (isLineActive)
            {
                lineTimer -= Time.deltaTime;
                if (lineTimer <= 0)
                {
                    lineRenderer.enabled = false;
                    isLineActive = false;
                }
            }
        }

        protected virtual void Shoot()
        {
            Vector3 startPoint = IsAiming ? Camera.main.transform.position : _shootPoint.transform.position;
            Vector3 direction = IsAiming ? cameraForward : _shootPoint.transform.forward;

            DrawShotLine(startPoint, direction);

            RaycastHit hit;
            if (Physics.Raycast(startPoint, direction, out hit, _range, _layerMask))
            {
                Debug.Log("Попал в: " + hit.collider.name);
                if (hit.collider.GetComponent<Health>() != null)
                {
                    ShotLiving(hit.collider.gameObject);
                }
            }
        }

        protected virtual void ShotLiving(GameObject liveObject)
        {
            Health health = liveObject.GetComponent<Health>();
            health.DamageBody((int)_damage);
        }

        private void DrawShotLine(Vector3 startPoint, Vector3 direction)
        {
            lineRenderer.SetPosition(0, _shootPoint.transform.position);
            lineRenderer.SetPosition(1, startPoint + direction * _range);

            lineRenderer.enabled = true;
            isLineActive = true;
            lineTimer = lineDuration;
        }
    }
}