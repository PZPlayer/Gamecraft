using Gamecraft.Player;
using UnityEngine;
using UnityEngine.Events;

public interface IPickable
{
    public void ShowCanPickUp();
    public bool PickUp();
}

namespace Gamecraft.Items
{
    public class DropOfWater : HealItem
    {
        [SerializeField] private GameObject _mesh;
        [SerializeField] private float _healRadius;
        [SerializeField] private int _healAmount;
        [SerializeField] private ParticleSystem _partcls;

        private void OnEnable()
        {
            animator = GetComponent<Animator>();
            _mesh.SetActive(true);
            animator.SetTrigger("Restart");
            
        }

        public void Disapear()
        {
            _mesh.SetActive(false);
            _partcls.Play();
        }

        protected override GameObject GetObjectOnScene()
        {
            return GameManager.Instance.DropOfWater;
        }

        public override bool Use()
        {
            base.Use();
            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _healRadius);
            foreach (var collider in hitColliders)
            {
                Health health = collider.GetComponent<Health>();
                if (health != null)
                {
                    health.HealBody(_healAmount);
                }
            }
            Invoke("Disapear", 0.1f);
            return false;
        }
    }
}
