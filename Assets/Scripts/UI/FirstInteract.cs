using System.Collections;
using UnityEngine;


namespace Gamecraft.UI
{
    public class FirstInteract : MonoBehaviour
    {
        [SerializeField] private GameObject[] _objectsAppear;

        private void Update()
        {
            if (Input.anyKey)
            {
                transform.GetComponent<Animator>().SetTrigger("Pressed");
            }
        }

        public void EndAnim()
        {
            foreach (GameObject obj in _objectsAppear)
            {
                obj.SetActive(true);
            }

            this.enabled = false;
        }
    }
}

