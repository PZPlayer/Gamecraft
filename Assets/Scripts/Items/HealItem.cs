using Gamecraft.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Gamecraft.Items
{
    public class HealItem : MonoBehaviour, IUsable, IPickable
    {
        [SerializeField] protected UnityEvent _onUse;
        [SerializeField] protected ItemOnScene _itemDesc;
        [SerializeField] protected float _pickUpRadius;
        [SerializeField] protected float _showUpRadius;
        [SerializeField] protected GameObject _pickMeCanvas;
        [SerializeField] protected bool _canBePicked;
        protected Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            ShowCanPickUp();
        }

        public void ShowCanPickUp()
        {
            if (_pickMeCanvas == null) return;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _showUpRadius);
            float distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);

            foreach (var collider in hitColliders)
            {
                if (distanceToCamera <= _showUpRadius && distanceToCamera > 5 )
                {
                    _pickMeCanvas.SetActive(true);
                    RotateCanvasToCamera();
                }
                else 
                {
                    _pickMeCanvas.SetActive(false);
                }

                float scale = distanceToCamera * 0.08f; 

                scale = Mathf.Clamp(scale, 0.01f, 1);

                if(_pickMeCanvas.transform.gameObject.activeSelf) _pickMeCanvas.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        private void RotateCanvasToCamera()
        {
            _pickMeCanvas.transform.LookAt(Camera.main.transform);
        }

        public bool PickUp()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _showUpRadius);

            foreach (var collider in hitColliders)
            {
                Inventory inv = collider.GetComponent<Inventory>();
                if (inv != null)
                {
                    inv.AddItem(_itemDesc);
                }
            }
            return false;
        }

        public virtual bool Use()
        {
            GameManager.Instance.PlayerAnimator.SetTrigger("Eat");
            _onUse.Invoke();
            return false;
        }
    }
}
