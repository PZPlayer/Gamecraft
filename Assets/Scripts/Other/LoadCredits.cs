using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gamecraft.Other
{
    public class LoadCredits : MonoBehaviour
    {
        public void BossDeafeated()
        {
            Invoke("SwitchScene", 4);
        }

        private void SwitchScene()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(2);
        }
    }
}

