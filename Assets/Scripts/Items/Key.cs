using Gamecraft.Player;
using UnityEngine;

namespace Gamecraft.Other
{
    public class Key : MonoBehaviour, IUsable
    {
        [SerializeField] private float _showUpRadius;
        [SerializeField] private GameObject _mesh;
        [SerializeField] private ItemOnScene _itemDesc;
        [SerializeField] private bool _ifMain;

        void Update()
        {
            if(!_ifMain) PickUp();
        }

        private void Disapear()
        {
            _mesh.SetActive(false);  
        }

        

        public bool PickUp()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _showUpRadius);

            foreach (var collider in hitColliders)
            {
                Inventory inv = collider.GetComponent<Inventory>();
                if (inv != null && Input.GetKeyDown(KeyCode.E))
                {
                    if (_itemDesc.ItemGameObject == null) _itemDesc.ItemGameObject = GameManager.Instance.Key;
                    bool can = inv.AddItem(_itemDesc);
                    if (can) Destroy(gameObject);
                }
            }
            return false;
        }

        public bool Use()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _showUpRadius);

            foreach (Collider collider in hitColliders)
            {
                if (collider.GetComponent<Door>() != null)
                {
                    collider.GetComponent<Door>().Open();
                    Disapear();
                    return false;
                }
            }
            return true;
        }
    }
}
