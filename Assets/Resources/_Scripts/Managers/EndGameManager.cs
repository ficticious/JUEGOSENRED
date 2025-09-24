using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class EndGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int killsToWin = 5;

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("kills"))
        {
            int kills = (int)targetPlayer.CustomProperties["kills"];

            if (kills >= killsToWin)
            {
                if (targetPlayer == PhotonNetwork.LocalPlayer)
                {
                    FindObjectOfType<WinScreen>().ShowWinPanel();
                }
                else
                {
                    FindObjectOfType<GameOverScreen>().ShowLosePanel();
                }
            }
        }
    }
}
