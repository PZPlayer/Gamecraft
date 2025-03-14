using Gamecraft.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUsable
{
    public void Use();
}

namespace Gamecraft.Guns
{
    public class IdleGun : MonoBehaviour, IUsable
    {
        public GameObject cube; // DELETE THIS AFTER PLAYTEST
        [SerializeField] protected Transform _shootPoint;
        [SerializeField] protected int _maxAmmo;
        [SerializeField] protected float _fireRate;
        [SerializeField] protected bool _ifAuto;
        [SerializeField] protected float _damage;
        [SerializeField] protected float _range;
        [SerializeField] protected LayerMask _layerMask;
        public bool isAiming;
        private float coolDown;

        public void Use()
        {
            Shoot();
        }

        private void Update()
        {
            coolDown += Time.deltaTime;
        }

        protected virtual void Shoot()
        {
            if(coolDown < _fireRate) return;
            coolDown = 0;
            RaycastHit hit;
            if (!isAiming)
            {
                if (Physics.Raycast(_shootPoint.transform.position, _shootPoint.transform.forward, out hit, _range, _layerMask))
                {
                    Debug.Log("Попал в: " + hit.collider.name);
                    Instantiate(cube, hit.point, Quaternion.identity);
                }
            }
            else
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, _range, _layerMask))
                {
                    Debug.Log("Попал в: " + hit.collider.name);
                    Instantiate(cube, hit.point, Quaternion.identity);
                }
            }
        }
    }
}
