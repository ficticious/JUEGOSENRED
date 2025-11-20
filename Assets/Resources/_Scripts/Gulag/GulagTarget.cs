using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GulagTarget : MonoBehaviourPunCallbacks
{
    private bool hasBeenHit = false;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        hasBeenHit = false;
    }

    public void Hit(int shooterViewID)
    {
        if (hasBeenHit) return;
        hasBeenHit = true;

        if (PhotonNetwork.IsMasterClient)
        {
            GulagManager.Instance.OnTargetHit(shooterViewID);
        }
    }
}
