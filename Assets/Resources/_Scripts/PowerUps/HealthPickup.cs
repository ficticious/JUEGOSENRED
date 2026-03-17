using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : PickUp
{
    [Header("Heal")]
    [SerializeField] private float healAmount;

    protected override void OnPickup(GameObject player)
    {
        PhotonView pv = player.GetComponent<PhotonView>();

        if (pv != null && pv.IsMine)
        {
            Health playerHealth = player.GetComponent<Health>();

            if (playerHealth != null && !playerHealth.IsDead)
            {
                playerHealth.Heal(healAmount);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
