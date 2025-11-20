using System.Collections;
using UnityEngine;
using Photon.Pun;


public class GulagTarget : MonoBehaviourPunCallbacks
{
    private bool hasBeenHit = false;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        hasBeenHit = false;
        Debug.Log("[TARGET] Target activado y listo");
    }

    public void Hit(int shooterViewID)
    {
        Debug.Log($"[TARGET] Hit por ViewID: {shooterViewID}");

        if (hasBeenHit) return;

        if (pv != null)
        {
            pv.RPC("RPC_RegisterHit", RpcTarget.MasterClient, shooterViewID);
        }
        else
        {
            RegisterHit(shooterViewID);
        }
    }

    [PunRPC]
    private void RPC_RegisterHit(int shooterViewID)
    {
        if (hasBeenHit) return;

        RegisterHit(shooterViewID);
    }

    private void RegisterHit(int shooterViewID)
    {
        if (hasBeenHit) return;

        hasBeenHit = true;
        Debug.Log($"[TARGET] Registrando hit de ViewID: {shooterViewID}");

        if (PhotonNetwork.IsMasterClient)
        {
            Minigame minigame = FindObjectOfType<Minigame>();
            if (minigame != null)
            {
                minigame.OnPlayerHitTarget(shooterViewID);
            }
            else
            {
                Debug.LogError("[TARGET] No se encontró Minigame!");
            }
        }
    }
}