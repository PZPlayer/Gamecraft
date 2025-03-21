using Gamecraft.Player;
using System.Collections;
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
        public bool isAiming;
        private float coolDown;
        private Vector3 cameraForward;

        [SerializeField] private float lineDistance = 10f;
        [SerializeField] private float lineDuration = 0.2f;
        [SerializeField] private Material lineMaterial;
        [SerializeField] private float lineWidth = 0.05f;

        private LineRenderer lineRenderer;
        private bool isLineActive = false;
        private float lineTimer = 0f;

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
            Shoot();
        }

        private void Update()
        {
            coolDown += Time.deltaTime;

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
            if (coolDown < _fireRate) return;
            coolDown = 0;

            Vector3 startPoint = isAiming ? Camera.main.transform.position : _shootPoint.transform.position;
            Vector3 direction = isAiming ? cameraForward : _shootPoint.transform.forward;

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