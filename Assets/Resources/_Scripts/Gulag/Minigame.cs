using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame : MonoBehaviourPun
{
    private Health playerA;
    private Health playerB;

    private int scoreA = 0;
    private int scoreB = 0;

    public int targetsToWin = 5;

    public void Init(Health a, Health b)
    {
        playerA = a;
        playerB = b;
    }

    public void OnPlayerHitTarget(int playerViewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (playerViewID == playerA.photonView.ViewID)
            scoreA++;
        else if (playerViewID == playerB.photonView.ViewID)
            scoreB++;

        CheckWinner();
    }

    private void CheckWinner()
    {
        if (scoreA >= targetsToWin)
            GulagManager.Instance.ReportWinner(playerA);

        if (scoreB >= targetsToWin)
            GulagManager.Instance.ReportWinner(playerB);
    }
}
