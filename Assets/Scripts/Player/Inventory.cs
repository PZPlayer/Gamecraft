using System;
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
        [SerializeField] private Sprite _selectedSprite;
        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private AudioSource _pickupSound;
        [SerializeField] private AudioSource _changeItemSound;
        private int selected = 0;


        void Start ()
        {
            UpdateQuickInv();
            GameManager.Instance.AudioEffects.Add(_pickupSound);
            GameManager.Instance.AudioEffects.Add(_changeItemSound);
        }

        public void ChangeItem(int index)
        {
            if ((index >= _item.Length) || index == selected)
            {
                HideAllItems();
                ResetIcons();
                selected = 0;
                return;
            }
            _changeItemSound.Play();
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
            bool ifCastItem = _item[selected].ItemGameObject.GetComponent<IUsable>().Use();

            if(!ifCastItem)
            {
                UpdateItem();
                LooseItem(selected);
                UpdateQuickInv();
            }
        }

        public bool AddItem(ItemOnScene item)
        {
            UpdateItem();
            UpdateQuickInv();
            foreach (ItemOnScene itemOnScene in _item)
            {
                if (itemOnScene.SlotIndex == 0) continue;
                if(itemOnScene.ItemInfo == null)
                {
                    item.SlotIndex = itemOnScene.SlotIndex;
                    _item[itemOnScene.SlotIndex] = item;
                    UpdateItem();
                    UpdateQuickInv();
                    _pickupSound.Play();
                    return true;
                }
            }
            UpdateItem();
            UpdateQuickInv();
            return false;
        }

        private void LooseItem(int index)
        {
            _item[index].ItemGameObject.SetActive(false);
            _item[index].ItemGameObject = null;
            _item[index].ItemInfo = null;
        }

        private void UpdateItem()
        {
            ResetIcons();
            HideAllItems();

            switch (selected)
            {
                case 1:
                    _firstInvImage.transform.parent.GetComponentInParent<Image>().sprite = _selectedSprite;
                    print("SELECTED1" + _firstInvImage.transform.parent.transform.parent.GetComponentInParent<Image>());
                    break;
                case 2:
                    _secondInvImage.transform.parent.GetComponentInParent<Image>().sprite = _selectedSprite;
                    print("SELECTED2");
                    break;
                case 3:
                    _thirdInvImage.transform.parent.GetComponentInParent<Image>().sprite = _selectedSprite;
                    print("SELECTED3");
                    break;
                default:
                    ResetIcons();
                    break;
            }

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
                switch (itm.SlotIndex)
                {
                    case 1:
                        _firstInvImage.sprite = itm.ItemInfo != null ? itm.ItemInfo.Image : null;
                        break;
                    case 2:
                        _secondInvImage.sprite = itm.ItemInfo != null ? itm.ItemInfo.Image : null;
                        break;
                    case 3:
                        _thirdInvImage.sprite = itm.ItemInfo != null ? itm.ItemInfo.Image : null;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ResetIcons()
        {
            _firstInvImage.transform.parent.GetComponentInParent<Image>().sprite = _normalSprite;
            _secondInvImage.transform.parent.GetComponentInParent<Image>().sprite = _normalSprite;
            _thirdInvImage.transform.parent.GetComponentInParent<Image>().sprite = _normalSprite;
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

