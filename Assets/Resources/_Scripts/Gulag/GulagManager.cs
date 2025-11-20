using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GulagManager : MonoBehaviourPunCallbacks
{
    public static GulagManager Instance;

    [Header("References")]
    public GulagArena arena;
    public GulagQueue queue;
    public GameObject minigamePrefab;

    [Header("Timing")]
    public float spectatorDelayBeforeGulag = 3f;
    public float gulagWaitTimeout = 15f;
    public float loserRespawnDelay = 10f;

    private Minigame currentMinigame;
    private PhotonView pv;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        pv = GetComponent<PhotonView>();
    }

    public void RequestGulagEntry(Health player)
    {
        if (!player.photonView.IsMine) return;

        GulagPlayerHandler.StartSpectatorDelay(player, spectatorDelayBeforeGulag, pv);
    }

    [PunRPC]
    private void RPC_AddPlayerToGulag(int playerViewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        queue.AddPlayer(playerViewID, pv, arena, gulagWaitTimeout);
    }

    [PunRPC]
    private void RPC_TeleportToWaiting(int viewID)
    {
        GulagPlayerHandler.TeleportToWaiting(viewID, arena.gulagSpawnPoints[0]);
    }

    [PunRPC]
    private void RPC_StartDuel(int viewID1, int viewID2)
    {
        Health player1 = PhotonView.Find(viewID1)?.GetComponent<Health>();
        Health player2 = PhotonView.Find(viewID2)?.GetComponent<Health>();

        if (player1 == null || player2 == null)
        {
            Debug.LogError("[GULAG] Jugadores no encontrados");
            return;
        }

        arena.StartDuel(player1, player2);

        if (PhotonNetwork.IsMasterClient)
        {
            currentMinigame = GulagMinigameSpawner.SpawnMinigame(minigamePrefab, player1, player2);
        }
    }

    public void OnTargetHit(int shooterViewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GulagDuelFinisher.FinishDuel(shooterViewID, currentMinigame, pv, loserRespawnDelay);

        if (currentMinigame != null)
        {
            PhotonNetwork.Destroy(currentMinigame.gameObject);
            currentMinigame = null;
        }

        queue.ProcessWaitingQueue(pv);
    }

    [PunRPC]
    private void RPC_DuelFinished(int winnerViewID, int loserViewID, float loserDelay)
    {
        GulagDuelFinisher.HandleDuelFinished(winnerViewID, loserViewID, loserDelay, arena);
    }

    [PunRPC]
    private void RPC_WaitInSpectatorWithTimeout(int playerViewID)
    {
        Health player = PhotonView.Find(playerViewID)?.GetComponent<Health>();
        if (player != null && player.photonView.IsMine)
        {
            GulagPlayerHandler.WaitWithTimeout(player, gulagWaitTimeout, queue, pv);
        }
    }
}
