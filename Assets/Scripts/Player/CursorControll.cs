using UnityEngine;
using UnityEngine.UI;


namespace Hydra.Player
{
    public class CursorControll : MonoBehaviour
    {
        [SerializeField] private GameObject _cursor;
        void Start ()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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