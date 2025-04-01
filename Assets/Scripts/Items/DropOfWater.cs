using Gamecraft.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Gamecraft.Items
{
    public class DropOfWater : HealItem
    {
        [SerializeField] private GameObject _mesh;
        [SerializeField] private float _healRadius;
        [SerializeField] private int _healAmount;

        public void Disapear()
        {
            _mesh.SetActive(false);
        }

        public override bool Use()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _healRadius);

            foreach (var collider in hitColliders)
            {
                Health health = collider.GetComponent<Health>();
                if (health != null)
                {
                    health.HealBody(_healAmount);
                }
            }
            Disapear();
            base.Use();
            return false;
        }
    }
}
