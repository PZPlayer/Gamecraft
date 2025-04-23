using UnityEngine;
using UnityEngine.Events;

namespace Gamecraft.Other
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private bool _locked;
        [SerializeField] private Rigidbody _rb;
        public UnityEvent OnOpen;

        void Start ()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody>();  
            if (_locked) _rb.freezeRotation = true;
        }
        
        public void Open()
        {
            _locked = false;
            _rb.freezeRotation = false;
            OnOpen.Invoke();
        }
    }
}
