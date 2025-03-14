using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Gamecraft.Player
{
    [Serializable]
    public struct DashSetting
    {
        public float Power;
        public KeyCode Key;
    }

    public class Dash : MonoBehaviour
    {
        private Rigidbody rb;
        private bool canJump = true;

        [HideInInspector] public bool IfGrounded;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public IEnumerator Jump(Vector3 direction, float power)
        {
            if (!canJump || !IfGrounded) yield break;
            canJump = false;
            rb.AddForce(Vector3.up * 300, ForceMode.Impulse);
            yield return new WaitForSeconds(0.1f);
            rb.AddForce(direction * power, ForceMode.Impulse);
            StartCoroutine(WaitBeforeJump(1));
        }

        private IEnumerator WaitBeforeJump(float time)
        {
            yield return new WaitForSeconds(time);
            canJump = true;
        }

    }
}
