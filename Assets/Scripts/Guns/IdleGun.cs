using Gamecraft.Player;
using UnityEngine;

public interface IUsable
{
    public bool Use();
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

        [SerializeField] private bool _ifMain;

        [SerializeField] private ItemOnScene _itemDesc;

        [SerializeField] private Animator _animator;

        [SerializeField] private Material lineMaterial;

        [SerializeField] private float lineDistance = 10f;
        [SerializeField] private float lineDuration = 0.2f;
        [SerializeField] private float lineWidth = 0.05f;
        [SerializeField] private float _showUpRadius;

        [SerializeField] private AudioSource _shootingSound;

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
            if(_shootingSound == null) _shootingSound = GetComponent<AudioSource>();
            if (_ifMain) GameManager.Instance.AudioEffects.Add(_shootingSound);
        }

        public bool Use()
        {
            if (coolDown < _fireRate) return true;
            coolDown = 0;
            _animator.SetTrigger("Shoot");
            Invoke("Shoot", 0.2f);
            return true;
        }

        private void Update()
        {
            IsAiming = GameManager.Instance.ifAiming;

            coolDown += Time.deltaTime;

            DrawLine();
            if(!_ifMain) PickUp();
        }

        public bool PickUp()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _showUpRadius);

            foreach (var collider in hitColliders)
            {
                Inventory inv = collider.GetComponent<Inventory>();
                if (inv != null && Input.GetKeyDown(KeyCode.E))
                {
                    if (_itemDesc.ItemGameObject == null) _itemDesc.ItemGameObject = GameManager.Instance.Gun;
                    bool can = inv.AddItem(_itemDesc);
                    if (can) Destroy(gameObject);
                }
            }
            return false;
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
            if(!_ifMain) return;

            Vector3 startPoint = IsAiming ? Camera.main.transform.position : _shootPoint.transform.position;
            Vector3 direction = IsAiming ? cameraForward : _shootPoint.transform.forward;

    
            RaycastHit hit;
            DrawShotLine(startPoint, direction);
            _shootingSound.Play();
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