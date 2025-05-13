using Gamecraft.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Hydra.Player
{
    public class CursorControll : MonoBehaviour
    {
        [SerializeField] private GameObject _cursor;
        [SerializeField] private GameObject _esc;
        [SerializeField] private bool _paused = false;
        void Start ()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update ()
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                if(_paused)
                {
                    SettingsManager.SETTINGS.UpdateAllSettings();
                    Continue();
                }
                else
                {
                    _esc.GetComponent<Animator>().SetTrigger("Open");
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    _paused = true;
                    Invoke("StopTime", 0.3f);
                }
                
            }
        }

        public void Continue()
        {
            _esc.GetComponent<Animator>().SetTrigger("Close");
            _paused = false;
            Time.timeScale = _paused == true ? 0 : 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UnStopTime()
        {
            _paused = false;
            Time.timeScale = _paused == true ? 0 : 1;
        }

        private void StopTime()
        {
            Time.timeScale = _paused == true ? 0 : 1;
        }

        public void CursorSetImage(Sprite img)
        {
            _cursor.transform.GetComponent<Image>().sprite = img;
            _cursor.SetActive(true);
        }

        public void HideCursor()
        {
            _cursor.SetActive(false);
        }
    }
}