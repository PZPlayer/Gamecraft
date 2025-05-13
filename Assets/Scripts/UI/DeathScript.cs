using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gamecraft.UI
{
    public class DeathScript : MonoBehaviour
    {
        private bool ifDead = false;


        private void Start()
        {
            Time.timeScale = 1.0f;
        }

        private void Update()
        {
            if (ifDead)
            {
                if (Input.anyKey)
                {
                    SceneManager.LoadScene(1);
                }
            }
        }

        public void PlayerDied()
        {
            ifDead = true;
            Time.timeScale = 0.3f;
        }
    }
}