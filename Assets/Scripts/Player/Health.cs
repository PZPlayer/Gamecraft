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
        private int curHealth;

        void Start ()
        {
            curHealth = _maxHealth;
        }

        public void DamageBody(int damage)
        {
            curHealth -= damage;
            OnTakeDamage.Invoke();
            if (curHealth < 0)
            {
                Death();
            }
        }

        public void HealBody(int heal)
        {
            curHealth = Mathf.Clamp(curHealth + heal, 0, _maxHealth);
            OnTakeHeal.Invoke();
        }

        private void Death()
        {
            OnDeath.Invoke();
        }
    }
}

