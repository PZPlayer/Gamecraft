using UnityEngine;
using UnityEngine.SceneManagement;


namespace Gamecraft.UI
{
    public class MenuManager : MonoBehaviour
    {
        public void LoadScene(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
