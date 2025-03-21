using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamecraft.Enemy
{
    public class EnemyGun : MonoBehaviour
    {
        [SerializeField] private GameObject[] _shootPoints;
        [SerializeField] private GameObject _bullet;
        [SerializeField] private GameObject _poolObjectKeep;

        [SerializeField] private int _maxAmmo;

        [SerializeField] private float _fireRate;
        [SerializeField] private float _reloadTime;
        [SerializeField] private float bulletLiveTimer;

        [SerializeField] private List<GameObject> poolOfBullets;

        private bool isReloading = false;

        private int currentAmmo;
        private int currentBullet = 0;

        private float reloadTimer;
        private float fireRateTimer;

        private void Start()
        {
            currentAmmo = _maxAmmo;
            InitPool();
        }

        private void Update()
        {
            fireRateTimer += Time.deltaTime;
        }

        private void InitPool()
        {
            for (int i = 0; i < _maxAmmo; i++)
            {
                GameObject bullet = Instantiate(_bullet, transform.position, Quaternion.identity);
                bullet.transform.parent = _poolObjectKeep.transform;
                poolOfBullets.Add(bullet);
                bullet.SetActive(false);
            }
        }

        private GameObject GetBullet()
        {
            if(currentBullet <= poolOfBullets.Count - 1)
            {
                return poolOfBullets[currentBullet];
            }
            return null;
        }

        private IEnumerator Reload()
        {
            isReloading = true;
            yield return new WaitForSeconds(_reloadTime);
            currentBullet = 0;
            isReloading = false;
        }

        private IEnumerator HideBullet(GameObject bullet)
        {
            yield return new WaitForSeconds(bulletLiveTimer);
            bullet.SetActive(false);
        }

        public virtual void Shoot()
        {
            if(fireRateTimer > _fireRate)
            {
                GameObject bullet = GetBullet();
                currentBullet++;
                if(bullet != null)
                {
                    int randomShootPoint = Random.Range(0, _shootPoints.Length -1);
                    bullet.SetActive(true);
                    bullet.transform.position = _shootPoints[randomShootPoint].transform.position;
                    bullet.transform.rotation = _shootPoints[randomShootPoint].transform.rotation;
                    HideBullet(bullet);
                }
                else
                {
                   if(!isReloading) StartCoroutine(Reload());
                }
                fireRateTimer = 0;
            }
        }
    }
}