using Gamecraft.Guns;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamecraft.Player
{
    [Serializable]
    public struct ItemOnScene
    {
        public GameObject ItemGameObject;
        public Item ItemInfo;
        public int SlotIndex;
    }
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private ItemOnScene[] _item;
        [SerializeField] private Image _firstInvImage;
        [SerializeField] private Image _secondInvImage;
        [SerializeField] private Image _thirdInvImage;
        private int selected = 0;


        void Start ()
        {
            UpdateQuickInv();
        }

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
            UpdateQuickInv();
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

        private void UpdateQuickInv()
        {
            foreach (ItemOnScene itm in _item)
            {
                if (itm.ItemInfo)
                {
                    switch (itm.SlotIndex)
                    {
                        case 0:
                            _firstInvImage.sprite = itm.ItemInfo.Image;
                            break;
                        case 1:
                            _secondInvImage.sprite = itm.ItemInfo.Image;
                            break;
                        case 2:
                            _thirdInvImage.sprite = itm.ItemInfo.Image;
                            break;
                    } 
                }
            }
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

