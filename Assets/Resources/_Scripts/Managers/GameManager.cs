using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int killsToWin = 5;

   // public GameObject player;

    private void Start()
    {
        
    }

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
                    StopAllPlayers();
                }
                else
                {
                    FindObjectOfType<GameOverScreen>().ShowLosePanel();
                    StopAllPlayers();
                }
            }
        }
    }

    public void StopAllPlayers()
    {
        var players = FindObjectsOfType<Movement>();
        var cameraPlayers = FindObjectsOfType<CameraMove>();

        foreach (var p in players)
        {
            p.enabled = false;
        }
        foreach (var d in cameraPlayers)
        {
            d.enabled = false;
        }

        //Connect.instance.kills = 0;
        //Connect.instance.deaths = 0;

    }
}
