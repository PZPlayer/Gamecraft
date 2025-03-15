using Cinemachine;
using Hydra.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamecraft.Player
{
    public class Aim : MonoBehaviour
    {
        [SerializeField] private GameObject _aimPoint;
        [SerializeField] private GameObject _lookPoint;
        [SerializeField] private GameObject _normalPoint;
        [SerializeField] private Sprite _aimImg;
        public bool isAiming = false;
        private CursorControll CursorControll;

        private void Start()
        {
            CursorControll = GetComponent<CursorControll>();
        }

        private void Update()
        {
            if (isAiming)
            {
                _lookPoint.transform.position = _aimPoint.transform.position;
            }
            else
            {
                _lookPoint.transform.position = _normalPoint.transform.position;
            }
        }

        public void OnStartAiming()
        {
            isAiming = true;
            CursorControll.CursorSetImage(_aimImg);
        }

        public void OnEndAiming()
        {
            isAiming = false;
            CursorControll.HideCursor();
        }
    }
}

