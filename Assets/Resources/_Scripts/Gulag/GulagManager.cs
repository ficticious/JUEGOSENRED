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
        StartSoloMinigame(player);
    }

    private void StartSoloMinigame(Health player)
    {
        GulagActive = true;
        photonView.RPC("RPC_StartSoloMinigame", RpcTarget.All, player.photonView.ViewID);
    }

    [PunRPC]
    private void RPC_StartSoloMinigame(int viewID)
    {
        Health player = PhotonView.Find(viewID).GetComponent<Health>();

        TeleportPlayer(player, gulagSpawnPoints[0]);

        player.ResetHealth();
        player.isDead = false;
        player.playerSetup.EnablePlayer();

        if (player.photonView.IsMine)
        {
            player.playerSetup.EnableLocalCamera(true);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject gm = PhotonNetwork.InstantiateRoomObject(
                MinigamePrefab.name,
                Vector3.zero,
                Quaternion.identity);
            currentMinigame = gm.GetComponent<Minigame>();
            currentMinigame.Init(player, null);
        }

       // UIGulag.Instance.ShowGulagUI(true);
    }

    public void ReportWinner(Health winner)
    {
        photonView.RPC("RPC_FinishGulag", RpcTarget.All, winner.photonView.ViewID);
        GulagActive = false;
    }

    [PunRPC]
    private void RPC_FinishGulag(int winnerViewID)
    {
        Health winner = PhotonView.Find(winnerViewID).GetComponent<Health>();
       // UIGulag.Instance.ShowGulagUI(false);

        winner.StartCoroutine(winner.RespawnAfterGulag());
    }

    private void TeleportPlayer(Health p, Transform spawn)
    {
        p.photonView.RPC("SetRespawnPosition", RpcTarget.All,
            spawn.position.x, spawn.position.y, spawn.position.z,
            spawn.rotation.x, spawn.rotation.y, spawn.rotation.z, spawn.rotation.w);
    }
}