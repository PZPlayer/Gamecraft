using Gamecraft.Guns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamecraft.Player
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private GameObject[] _item;
        private Aim aim;
        private int selected = 0;

        private void Start()
        {
            aim = GetComponent<Aim>();
        }

        public void ChangeItem(int index)
        {
            if (index >= _item.Length)
            {
                HideAllItems();
                return;
            }
            selected = index;
            UpdateItem();
        }

        public void UseItem()
        {
            if (selected == 0)
            {
                HideAllItems();
                return;
            }
            _item[selected].GetComponent<IdleGun>().isAiming = aim.isAiming;
            _item[selected].GetComponent<IUsable>().Use();
        }

        private void UpdateItem()
        {
            HideAllItems();
            _item[selected].SetActive(true);
        }

        private void HideAllItems()
        {
            foreach (GameObject obj in _item)
            {
                obj.SetActive(false);
            }
        }
    }
}

