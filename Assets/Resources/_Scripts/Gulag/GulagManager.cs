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

    private List<Health> waitingPlayers = new List<Health>();
    private Health currentWinner;

    private void Awake()
    {
        Instance = this;
    }

    public void AddPlayerToGulag(Health player)
    {
        if (!photonView.IsMine) return;

        waitingPlayers.Add(player);

        if (waitingPlayers.Count == 1)
        {
            photonView.RPC("RPC_TeleportToWaiting", RpcTarget.All, player.photonView.ViewID, 0);
        }
        else if (waitingPlayers.Count == 2)
        {
            StartGulagDuel();
        }
    }

    private void StartGulagDuel()
    {
        GulagActive = true;
        photonView.RPC("RPC_StartDuel", RpcTarget.All,
            waitingPlayers[0].photonView.ViewID,
            waitingPlayers[1].photonView.ViewID);
    }

    [PunRPC]
    private void RPC_TeleportToWaiting(int viewID, int spawnIndex)
    {
        Health player = PhotonView.Find(viewID).GetComponent<Health>();

        TeleportPlayer(player, gulagSpawnPoints[spawnIndex]);

        player.ResetHealth();
        player.isDead = false;
        player.playerSetup.EnablePlayer();

        if (player.photonView.IsMine)
        {
            player.playerSetup.EnableLocalCamera(true);
        }
    }

    [PunRPC]
    private void RPC_StartDuel(int viewID1, int viewID2)
    {
        Health player1 = PhotonView.Find(viewID1).GetComponent<Health>();
        Health player2 = PhotonView.Find(viewID2).GetComponent<Health>();

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
        }
        if (player2.photonView.IsMine)
        {
            player2.playerSetup.EnablePlayer();
            player2.playerSetup.EnableLocalCamera(true);
        }
    }

    public void OnTargetHit(int shooterViewID)
    {
        if (!PhotonNetwork.IsMasterClient || !GulagActive) return;

        Health winner = PhotonView.Find(shooterViewID).GetComponent<Health>();
        Health loser = null;

        foreach (Health player in waitingPlayers)
        {
            if (player.photonView.ViewID != shooterViewID)
            {
                loser = player;
                break;
            }
        }

        photonView.RPC("RPC_DuelFinished", RpcTarget.All,
            winner.photonView.ViewID,
            loser != null ? loser.photonView.ViewID : -1);

        waitingPlayers.Clear();
        GulagActive = false;
    }

    [PunRPC]
    private void RPC_DuelFinished(int winnerViewID, int loserViewID)
    {
        Health winner = PhotonView.Find(winnerViewID).GetComponent<Health>();

        foreach (GameObject wall in wallsToDisable)
        {
            wall.SetActive(true);
        }

        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }

        winner.StartCoroutine(winner.RespawnAfterGulag());

        if (loserViewID != -1)
        {
            Health loser = PhotonView.Find(loserViewID).GetComponent<Health>();
            StartCoroutine(RespawnLoserAfterDelay(loser));
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