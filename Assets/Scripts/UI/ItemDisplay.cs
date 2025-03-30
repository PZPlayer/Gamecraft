using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gamecraft.UI
{
    public class ItemDisplay : MonoBehaviour
    {
        [SerializeField] private Image _itemImage;
        [SerializeField] private Image _frameItemImage;
        [SerializeField] private Sprite _unSelected;
        [SerializeField] private Sprite _selected;

        public void UpdateInfo(Item item = null)
        {
            if(item == null)
            {
                _frameItemImage.sprite = _unSelected;
                _itemImage.gameObject.SetActive(false);
                return;
            }

        }
    }
}
