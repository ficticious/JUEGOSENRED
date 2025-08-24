using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpeed : PickUp
{
    [SerializeField] private float speedMultiplier;

    protected override void OnPickup(GameObject player)
    {
        Movement move = player.GetComponent<Movement>();

        if (move != null)
        {
            move.SpeedBoost(speedMultiplier);
        }
    }
}
