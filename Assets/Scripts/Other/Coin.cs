using UnityEngine;

namespace Gamecraft.Other
{
    public class Coin : MonoBehaviour
    {
        private AudioSource audioSourc;
        private bool ifActv = true;

        private void Start ()
        {
            audioSourc = GetComponent<AudioSource>();
            GameManager.Instance.AudioEffects.Add(audioSourc);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && ifActv)
            {
                GameManager.Instance.PlayerBody.transform.localScale += new Vector3(0, 0.1f, 0);
                audioSourc.Play();
                transform.GetComponent<Animator>().SetTrigger("Go");
                ifActv = false;
                Destroy(gameObject, 1f);
            }
        }
    }
}
