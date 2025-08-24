using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerUp : PickUp
{
    protected override void OnPickup(GameObject player)
    {
        Shield shield = player.GetComponentInChildren<Shield>(true);

        if (shield != null)
        {
            Debug.Log("ShieldPickUP");
            shield.Activate();
        }
    }
}
