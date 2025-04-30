using UnityEngine;
using UnityEngine.Events;

namespace Gamecraft.Other
{
    public class Train : MonoBehaviour
    {
        [SerializeField] private bool _ifLockedVans = true;
        [SerializeField] private GameObject _vans;
        [SerializeField] private GameObject _key;
        [SerializeField] private Transform _keySpawn;
        [SerializeField] private Animator _animator;

        private Vector3 oldPlayerPos;

        public UnityEvent OnEngineStart;


        public void UnlockVans() => _ifLockedVans = false;

        public void Restart()
        {
            GameManager.Instance.UIDeathAnimator.SetTrigger("Failure");
            Invoke("ThrowPlayerBack", 0.3f);
            Instantiate(_key, _keySpawn);
        }

        private void ThrowPlayerBack() => GameManager.Instance.Player.transform.position = oldPlayerPos;

        public void EngineStart()
        {
            OnEngineStart.Invoke();

            oldPlayerPos = GameManager.Instance.Player.transform.position;

            if (!_ifLockedVans)
            {
                _animator.SetBool("Unlocked", true);
                _vans.transform.parent = null;
            }

            _animator.SetTrigger("Start");
        }
    }
}

