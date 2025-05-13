using Gamecraft.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Gamecraft.Other
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private Image _heartImage;
        [SerializeField] private Animator _anmtr;

        public void ChangeHP()
        {
            _heartImage.fillAmount = _health.CurentHealth / _health.MaxHealth;
            _anmtr.SetTrigger("Change");
        }
    }
}
