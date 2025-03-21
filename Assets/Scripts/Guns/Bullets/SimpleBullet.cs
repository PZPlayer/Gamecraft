using Gamecraft.Player;
using UnityEngine;

namespace Gamecraft.Guns.Bullets
{
    public class SimpleBullet : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _damage;

        private void Update()
        {
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.GetComponent<Health>())
            {
                other.transform.GetComponent<Health>().DamageBody((int)_damage);
            }
            transform.gameObject.SetActive(false);
        }
    }
}

