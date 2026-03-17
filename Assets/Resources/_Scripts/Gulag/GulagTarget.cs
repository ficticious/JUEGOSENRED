using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GulagTarget : MonoBehaviourPunCallbacks
{
    //private bool hasBeenHit = false;
    //private PhotonView pv;

    //private void Awake()
    //{
    //    pv = GetComponent<PhotonView>();
    //}

    //private void Start()
    //{
    //    gameObject.SetActive(false);
    //}

    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //    hasBeenHit = false;
    //}

    //public void Hit(int shooterViewID)
    //{
    //    if (hasBeenHit) return;

    //    pv.RPC("RPC_RegisterHit", RpcTarget.MasterClient, shooterViewID);
    //}

    //[PunRPC]
    //private void RPC_RegisterHit(int shooterViewID)
    //{
    //    if (hasBeenHit) return;
    //    hasBeenHit = true;

    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        GulagManager.Instance.OnTargetHit(shooterViewID);
    //    }
    //}
}
