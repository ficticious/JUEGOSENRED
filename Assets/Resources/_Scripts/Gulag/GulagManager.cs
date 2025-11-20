using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GulagManager : MonoBehaviourPunCallbacks
{
    public static GulagManager Instance;

    [Header("Gulag Settings")]
    public Transform[] gulagSpawnPoints;
    public GameObject[] wallsToDisable;
    public GameObject targetObject;
    public float loserRespawnDelay = 10f;

    public bool GulagActive = false;

    private List<int> waitingPlayerViewIDs = new List<int>();
    private PhotonView pv;

    private void Awake()
    {
        Instance = this;
        pv = GetComponent<PhotonView>();
    }

    public void RequestGulagEntry(int playerViewID)
    {
        pv.RPC("RPC_AddPlayerToGulag", RpcTarget.MasterClient, playerViewID);
    }

    [PunRPC]
    private void RPC_AddPlayerToGulag(int playerViewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        waitingPlayerViewIDs.Add(playerViewID);

        Debug.Log($"Jugador {playerViewID} agregado al gulag. Total esperando: {waitingPlayerViewIDs.Count}");

        if (waitingPlayerViewIDs.Count == 1)
        {
            pv.RPC("RPC_TeleportToWaiting", RpcTarget.All, playerViewID, 0);
        }
        else if (waitingPlayerViewIDs.Count >= 2)
        {
            StartGulagDuel();
        }
    }

    private void StartGulagDuel()
    {
        GulagActive = true;
        pv.RPC("RPC_StartDuel", RpcTarget.All,
            waitingPlayerViewIDs[0],
            waitingPlayerViewIDs[1]);
    }

    [PunRPC]
    private void RPC_TeleportToWaiting(int viewID, int spawnIndex)
    {
        Health player = PhotonView.Find(viewID).GetComponent<Health>();
        if (player == null)
        {
            Debug.LogError($"No se encontró jugador con ViewID {viewID}");
            return;
        }

        Debug.Log($"Teletransportando jugador {viewID} al spawn {spawnIndex}");

        TeleportPlayer(player, gulagSpawnPoints[spawnIndex]);

        player.ResetHealth();
        player.isDead = false;
        player.playerSetup.EnablePlayer();

        if (player.photonView.IsMine)
        {
            player.playerSetup.EnableLocalCamera(true);

            if (SpectatorCameraManager.Instance != null)
            {
                SpectatorCameraManager.Instance.DisableSpectator();
            }
        }
    }

    [PunRPC]
    private void RPC_StartDuel(int viewID1, int viewID2)
    {
        Health player1 = PhotonView.Find(viewID1).GetComponent<Health>();
        Health player2 = PhotonView.Find(viewID2).GetComponent<Health>();

        if (player1 == null || player2 == null)
        {
            Debug.LogError("No se encontraron los jugadores para el duelo");
            return;
        }

        Debug.Log($"Iniciando duelo entre {viewID1} y {viewID2}");

        foreach (GameObject wall in wallsToDisable)
        {
            wall.SetActive(false);
        }

        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }

        player1.ResetHealth();
        player2.ResetHealth();
        player1.isDead = false;
        player2.isDead = false;

        if (player1.photonView.IsMine)
        {
            player1.playerSetup.EnablePlayer();
            player1.playerSetup.EnableLocalCamera(true);

            if (SpectatorCameraManager.Instance != null)
            {
                SpectatorCameraManager.Instance.DisableSpectator();
            }
        }
        if (player2.photonView.IsMine)
        {
            player2.playerSetup.EnablePlayer();
            player2.playerSetup.EnableLocalCamera(true);

            if (SpectatorCameraManager.Instance != null)
            {
                SpectatorCameraManager.Instance.DisableSpectator();
            }
        }
    }

    public void OnTargetHit(int shooterViewID)
    {
        if (!PhotonNetwork.IsMasterClient || !GulagActive) return;

        Health winner = PhotonView.Find(shooterViewID).GetComponent<Health>();
        if (winner == null) return;

        Health loser = null;

        foreach (int viewID in waitingPlayerViewIDs)
        {
            if (viewID != shooterViewID)
            {
                loser = PhotonView.Find(viewID).GetComponent<Health>();
                break;
            }
        }

        pv.RPC("RPC_DuelFinished", RpcTarget.All,
            winner.photonView.ViewID,
            loser != null ? loser.photonView.ViewID : -1);

        waitingPlayerViewIDs.Clear();
        GulagActive = false;
    }

    [PunRPC]
    private void RPC_DuelFinished(int winnerViewID, int loserViewID)
    {
        Health winner = PhotonView.Find(winnerViewID).GetComponent<Health>();
        if (winner == null) return;

        foreach (GameObject wall in wallsToDisable)
        {
            wall.SetActive(true);
        }

        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }

        if (winner.photonView.IsMine)
        {
            winner.StartCoroutine(winner.RespawnAfterGulag());
        }

        if (loserViewID != -1)
        {
            Health loser = PhotonView.Find(loserViewID).GetComponent<Health>();
            if (loser != null && loser.photonView.IsMine)
            {
                StartCoroutine(RespawnLoserAfterDelay(loser));
            }
        }
    }

    private IEnumerator RespawnLoserAfterDelay(Health loser)
    {
        yield return new WaitForSeconds(loserRespawnDelay);

        if (loser != null && loser.photonView.IsMine)
        {
            loser.StartCoroutine(loser.RespawnAfterGulag());
        }
    }

    private void TeleportPlayer(Health p, Transform spawn)
    {
        p.photonView.RPC("SetRespawnPosition", RpcTarget.All,
            spawn.position.x, spawn.position.y, spawn.position.z,
            spawn.rotation.x, spawn.rotation.y, spawn.rotation.z, spawn.rotation.w);
    }
}