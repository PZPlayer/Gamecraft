using UnityEngine;
using UnityEngine.Events;

namespace Gamecraft.Items
{
    public class HealItem : MonoBehaviour, IUsable
    {
        [SerializeField] private UnityEvent OnUse;
        private Animator animator;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        public virtual bool Use()
        {
            OnUse.Invoke();
            return false;
        }
    }
}
