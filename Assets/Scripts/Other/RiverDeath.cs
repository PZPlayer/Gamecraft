using UnityEngine;
using Gamecraft.Player;

namespace Gamecraft.Other
{
    public class RiverDeath : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Health otherHealth = other.GetComponent<Health>();
            if (otherHealth != null)
            {
                otherHealth.DamageBody(2000000);
            }
        }
    }
}