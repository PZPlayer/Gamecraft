using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gamecraft.Other
{
    public class ComicsManager : MonoBehaviour
    {
        [SerializeField] private AudioClip[] _sounds;
        [SerializeField] private AudioSource _audioSource;

        public void PlaySound(int index)
        {
            _audioSource.PlayOneShot(_sounds[index]);
        }

        public void SwitchScene()
        {
            SceneManager.LoadScene(1);
        }
    }
}
