using UnityEngine;
using UnityEngine.Events;

namespace Gamecraft.Player
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnTakeDamage;
        [SerializeField] private UnityEvent OnTakeHeal;
        [SerializeField] private UnityEvent OnDeath;

        [SerializeField] private int _maxHealth;
        [SerializeField] private int _curHealth;

        public float CurentHealth { get { return _curHealth; } }
        public float MaxHealth { get { return _maxHealth; } }

        void Start ()
        {
            _curHealth = _maxHealth;
        }

        public void DamageBody(int damage)
        {
            _curHealth -= damage;
            OnTakeDamage.Invoke();
            if (_curHealth < 0)
            {
                Death();
            }
        }

        public void HealBody(int heal)
        {
            _curHealth = Mathf.Clamp(_curHealth + heal, 0, _maxHealth);
            OnTakeHeal.Invoke();
        }

        private void Death()
        {
            OnDeath.Invoke();
        }
    }
}

