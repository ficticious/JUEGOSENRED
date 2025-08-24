using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : PickUp
{
    [SerializeField] private float healAmount;

    protected override void OnPickup(GameObject player)
    {
        Health playerHealth = player.GetComponent<Health>();

        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
        }
    }
}
