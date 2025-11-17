using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GulagManager : MonoBehaviourPunCallbacks
{
    public static GulagManager Instance;

    [Header("Gulag Settings")]
    public Transform[] gulagSpawnPoints;
    public GameObject MinigamePrefab;

    public bool GulagActive = false;
    private List<Health> waitingPlayers = new List<Health>();

    private Minigame currentMinigame;

    private void Awake()
    {
        Instance = this;
    }

    public void AddPlayerToGulag(Health player)
    {
        waitingPlayers.Add(player);

        if (waitingPlayers.Count == 1)
        {
            TeleportToWaitingArea(player);
        }
        else if (waitingPlayers.Count == 2)
        {
            StartMinigame(waitingPlayers[0], waitingPlayers[1]);
        }
    }

    private void StartMinigame(Health p1, Health p2)
    {
        GulagActive = true;

        photonView.RPC("RPC_StartMinigame", RpcTarget.All,
            p1.photonView.ViewID,
            p2.photonView.ViewID);
    }

    [PunRPC]
    private void RPC_StartMinigame(int viewA, int viewB)
    {
        Health playerA = PhotonView.Find(viewA).GetComponent<Health>();
        Health playerB = PhotonView.Find(viewB).GetComponent<Health>();

        // Teleport a posiciones del minijuego
        TeleportPlayer(playerA, gulagSpawnPoints[0]);
        TeleportPlayer(playerB, gulagSpawnPoints[1]);

        // Activar players en modo minijuego
        playerA.ResetHealth();
        playerB.ResetHealth();
        playerA.isDead = false;
        playerB.isDead = false;

        playerA.playerSetup.EnablePlayer();
        playerB.playerSetup.EnablePlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject gm = PhotonNetwork.InstantiateRoomObject(
                MinigamePrefab.name,
                Vector3.zero,
                Quaternion.identity);

            currentMinigame = gm.GetComponent<Minigame>();
            currentMinigame.Init(playerA, playerB);
        }

        UIGulag.Instance.ShowGulagUI(true);
    }

    public void ReportWinner(Health winner)
    {
        photonView.RPC("RPC_FinishGulag", RpcTarget.All, winner.photonView.ViewID);

        waitingPlayers.Clear();
        GulagActive = false;
    }

    [PunRPC]
    private void RPC_FinishGulag(int winnerViewID)
    {
        Health winner = PhotonView.Find(winnerViewID).GetComponent<Health>();

        UIGulag.Instance.ShowGulagUI(false);

        // Ganador vuelve al mapa
        winner.StartCoroutine(winner.RespawnAfterGulag());
    }

    private void TeleportToWaitingArea(Health p)
    {
        p.playerSetup.DisablePlayer();
        p.playerSetup.EnableLocalCamera(false);
    }

    private void TeleportPlayer(Health p, Transform spawn)
    {
        p.photonView.RPC("SetRespawnPosition", RpcTarget.All,
            spawn.position.x, spawn.position.y, spawn.position.z,
            spawn.rotation.x, spawn.rotation.y, spawn.rotation.z, spawn.rotation.w);
    }
}
