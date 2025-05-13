using Invector.vCharacterController;
using UnityEngine;

namespace Gamecraft.Player
{
    public class PlayerFreeze : MonoBehaviour
    {
        [SerializeField] private vThirdPersonController _playerController;
        [SerializeField] private vThirdPersonInput _playerInput;

        public void Freeze()
        {
            _playerController.enabled = false;
            _playerInput.enabled = false;
            _playerInput.transform.GetComponent<Rigidbody>().isKinematic = true;
            GameManager.Instance.PlayerAnimator.SetBool("Move", false);
            GameManager.Instance.CameraContoller.enabled = false;
        }

        public void UnFreeze()
        {
            _playerController.enabled = true;
            _playerInput.enabled = true;
            _playerInput.transform.GetComponent<Rigidbody>().isKinematic = false;
            GameManager.Instance.CameraContoller.enabled = true;
        }
    }
}
