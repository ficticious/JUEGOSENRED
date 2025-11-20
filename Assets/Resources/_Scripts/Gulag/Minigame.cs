using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Minigame : MonoBehaviourPunCallbacks
{
    private Health playerA;
    private Health playerB;
    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void Initialize(Health player1, Health player2)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        playerA = player1;
        playerB = player2;

        Debug.Log($"[MINIGAME] Inicializado con {player1.gameObject.name} vs {player2.gameObject.name}");

        pv.RPC("RPC_InitializeAll", RpcTarget.All, player1.photonView.ViewID, player2.photonView.ViewID);
    }

    [PunRPC]
    private void RPC_InitializeAll(int player1ViewID, int player2ViewID)
    {
        playerA = PhotonView.Find(player1ViewID)?.GetComponent<Health>();
        playerB = PhotonView.Find(player2ViewID)?.GetComponent<Health>();

        Debug.Log($"[MINIGAME] Players asignados en cliente");
    }

    public void OnPlayerHitTarget(int shooterViewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log($"[MINIGAME] Target golpeado por ViewID: {shooterViewID}");

        GulagManager.Instance.OnTargetHit(shooterViewID);
    }

    public Health GetPlayerA()
    {
        return playerA;
    }

    public Health GetPlayerB()
    {
        return playerB;
    }
}