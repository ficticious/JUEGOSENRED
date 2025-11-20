using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GulagMinigame : MonoBehaviourPun
{
    private Health p1;
    private Health p2;

    private int p1Points;
    private int p2Points;

    public int pointsToWin = 3;

    public void StartMinigame(Health _p1, Health _p2)
    {
        p1 = _p1;
        p2 = _p2;

        p1Points = 0;
        p2Points = 0;

        EnableTargets(true);
    }

    public void RegisterHit(int shooterViewID)
    {
        PhotonView pv = PhotonView.Find(shooterViewID);

        if (pv.Owner.ActorNumber == p1.photonView.Owner.ActorNumber)
            p1Points++;
        else
            p2Points++;

        if (p1Points >= pointsToWin)
            photonView.RPC("RPC_Finish", RpcTarget.All, p1.photonView.ViewID, p2.photonView.ViewID);

        else if (p2Points >= pointsToWin)
            photonView.RPC("RPC_Finish", RpcTarget.All, p2.photonView.ViewID, p1.photonView.ViewID);
    }

    [PunRPC]
    private void RPC_Finish(int winnerID, int loserID)
    {
        EnableTargets(false);

        Health winner = PhotonView.Find(winnerID).GetComponent<Health>();
        Health loser = PhotonView.Find(loserID).GetComponent<Health>();

        GulagMatchManager.Instance.FinishMatch(winner, loser);
    }

    private void EnableTargets(bool enable)
    {
        foreach (GulagTarget t in GetComponentsInChildren<GulagTarget>())
            t.gameObject.SetActive(enable);
    }
}
