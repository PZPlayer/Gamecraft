using Gamecraft.Guns;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamecraft.Player
{
    [Serializable]
    public struct ItemOnScene
    {
        public GameObject ItemGameObject;
        public Item ItemInfo;
    }
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private ItemOnScene[] _item;
        private int selected = 0;

        public void ChangeItem(int index)
        {
            if ((index >= _item.Length) || index == selected)
            {
                HideAllItems();
                selected = 0;
                return;
            }
            selected = index;
            print(selected);
            UpdateItem();
        }

        public void UseItem()
        {
            if (selected == 0 || _item[selected].ItemGameObject == null)
            {
                HideAllItems();
                return;
            }
            _item[selected].ItemGameObject.GetComponent<IUsable>().Use();
        }

        private void UpdateItem()
        {
            HideAllItems();
            if (_item[selected].ItemGameObject == null)
            {
                HideAllItems();
                return;
            }
            _item[selected].ItemGameObject.SetActive(true);
        }

        private void HideAllItems()
        {
            foreach (ItemOnScene itm in _item)
            {
                GameObject obj = itm.ItemGameObject;   
                if (obj == null) continue;
                obj.SetActive(false);
            }
        }
    }
}

