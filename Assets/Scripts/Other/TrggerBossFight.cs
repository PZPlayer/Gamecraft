using Gamecraft.Player;
using Gamecraft.Enemy;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Gamecraft.Other
{
    public class TrggerBossFight : MonoBehaviour
    {
        [SerializeField] private Animator _trainAnimator;
        [SerializeField] private Animator _bossQuestAnimator;
        [SerializeField] private GameObject _boss;
        [SerializeField] private Transform _bossSpawnPosition;
        [SerializeField] private BossAI _bossAI;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _bossMusic;
        public UnityEvent OnSummon;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _trainAnimator.SetTrigger("Start");
                _bossQuestAnimator.SetTrigger("Start");
                _audioSource.clip = _bossMusic;
                _audioSource.Play();
                GameManager.Instance.CameraContoller.enabled = false;
                Camera.main.GetComponent<Animator>().enabled = true;
                Camera.main.GetComponent<Animator>().SetTrigger("CutScene");
                GameManager.Instance.Player.GetComponent<PlayerFreeze>().Freeze();  
                Invoke("SummonBoss", 9);
            }
        }

        private void SummonBoss()
        {
            OnSummon?.Invoke();
            _bossAI.enabled = true;
            GameManager.Instance.CameraContoller.enabled = true;
            Camera.main.GetComponent<Animator>().enabled = false;
            _bossQuestAnimator.enabled = false;
            _boss.transform.position = _bossSpawnPosition.position;
            GameManager.Instance.Player.GetComponent<PlayerFreeze>().UnFreeze();
            _agent.enabled = true;
            Destroy(gameObject);
        }
    }
}

