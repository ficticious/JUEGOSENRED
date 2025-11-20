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
        Debug.Log($"[LOCAL] RequestGulagEntry llamado para ViewID: {playerViewID}");
        pv.RPC("RPC_AddPlayerToGulag", RpcTarget.MasterClient, playerViewID);
    }

    [PunRPC]
    private void RPC_AddPlayerToGulag(int playerViewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log($"[MASTER] Jugador {playerViewID} agregado al gulag.");

        waitingPlayerViewIDs.Add(playerViewID);

        Debug.Log($"[MASTER] Total esperando: {waitingPlayerViewIDs.Count}");

        if (waitingPlayerViewIDs.Count == 1)
        {
            Debug.Log($"[MASTER] Primer jugador, enviando a sala de espera en spawn 0");
            pv.RPC("RPC_TeleportToWaiting", RpcTarget.All, playerViewID, 0);
        }
        else if (waitingPlayerViewIDs.Count == 2)
        {
            Debug.Log($"[MASTER] Segundo jugador detectado, iniciando duelo");
            StartGulagDuel();
        }
    }

    private void StartGulagDuel()
    {
        GulagActive = true;

        Debug.Log($"[MASTER] Iniciando duelo entre ViewID {waitingPlayerViewIDs[0]} y ViewID {waitingPlayerViewIDs[1]}");

        pv.RPC("RPC_StartDuel", RpcTarget.All,
            waitingPlayerViewIDs[0],
            waitingPlayerViewIDs[1]);
    }

    [PunRPC]
    private void RPC_TeleportToWaiting(int viewID, int spawnIndex)
    {
        Debug.Log($"[RPC] RPC_TeleportToWaiting - ViewID: {viewID}, Spawn: {spawnIndex}");

        Health player = PhotonView.Find(viewID).GetComponent<Health>();
        if (player == null)
        {
            Debug.LogError($"[ERROR] No se encontró jugador con ViewID {viewID}");
            return;
        }

        Debug.Log($"[RPC] Jugador encontrado: {player.gameObject.name}, teletransportando...");

        TeleportPlayer(player, gulagSpawnPoints[spawnIndex]);

        player.ResetHealth();
        player.isDead = false;
        player.playerSetup.EnablePlayer();

        if (player.photonView.IsMine)
        {
            Debug.Log($"[LOCAL] Es mi jugador, habilitando cámara");
            player.playerSetup.EnableLocalCamera(true);
        }
    }

    [PunRPC]
    private void RPC_StartDuel(int viewID1, int viewID2)
    {
        Debug.Log($"[RPC] RPC_StartDuel - ViewID1: {viewID1}, ViewID2: {viewID2}");

        Health player1 = PhotonView.Find(viewID1).GetComponent<Health>();
        Health player2 = PhotonView.Find(viewID2).GetComponent<Health>();

        if (player1 == null)
        {
            Debug.LogError($"[ERROR] No se encontró player1 con ViewID {viewID1}");
            return;
        }
        if (player2 == null)
        {
            Debug.LogError($"[ERROR] No se encontró player2 con ViewID {viewID2}");
            return;
        }

        Debug.Log($"[RPC] Jugadores encontrados: {player1.gameObject.name} y {player2.gameObject.name}");

        Debug.Log($"[RPC] Teletransportando {player1.gameObject.name} al spawn 0");
        TeleportPlayer(player1, gulagSpawnPoints[0]);

        Debug.Log($"[RPC] Teletransportando {player2.gameObject.name} al spawn 1");
        TeleportPlayer(player2, gulagSpawnPoints[1]);

        foreach (GameObject wall in wallsToDisable)
        {
            wall.SetActive(false);
        }
        Debug.Log($"[RPC] Paredes desactivadas");

        if (targetObject != null)
        {
            targetObject.SetActive(true);
            Debug.Log($"[RPC] Target activado");
        }

        player1.ResetHealth();
        player2.ResetHealth();
        player1.isDead = false;
        player2.isDead = false;

        if (player1.photonView.IsMine)
        {
            Debug.Log($"[LOCAL] Player1 es mío, habilitando");
            player1.playerSetup.EnablePlayer();
            player1.playerSetup.EnableLocalCamera(true);
        }
        if (player2.photonView.IsMine)
        {
            Debug.Log($"[LOCAL] Player2 es mío, habilitando");
            player2.playerSetup.EnablePlayer();
            player2.playerSetup.EnableLocalCamera(true);
        }

        Debug.Log($"[RPC] Duelo iniciado completamente");
    }

    public void OnTargetHit(int shooterViewID)
    {
        if (!PhotonNetwork.IsMasterClient || !GulagActive) return;

        Debug.Log($"[MASTER] Target golpeado por ViewID: {shooterViewID}");

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

        Debug.Log($"[MASTER] Ganador: {winner.gameObject.name}, Perdedor: {(loser != null ? loser.gameObject.name : "Ninguno")}");

        pv.RPC("RPC_DuelFinished", RpcTarget.All,
            winner.photonView.ViewID,
            loser != null ? loser.photonView.ViewID : -1);

        waitingPlayerViewIDs.Clear();
        GulagActive = false;
    }

    [PunRPC]
    private void RPC_DuelFinished(int winnerViewID, int loserViewID)
    {
        Debug.Log($"[RPC] Duelo finalizado - Ganador: {winnerViewID}, Perdedor: {loserViewID}");

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
            Debug.Log($"[LOCAL] Soy el ganador, respawneando");
            winner.StartCoroutine(winner.RespawnAfterGulag());
        }

        if (loserViewID != -1)
        {
            Health loser = PhotonView.Find(loserViewID).GetComponent<Health>();
            if (loser != null && loser.photonView.IsMine)
            {
                Debug.Log($"[LOCAL] Soy el perdedor, esperando {loserRespawnDelay}s");
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
        if (spawn == null)
        {
            Debug.LogError("[ERROR] Spawn point es NULL!");
            return;
        }

        Debug.Log($"[TELEPORT] Teletransportando a posición: {spawn.position}");

        p.photonView.RPC("SetRespawnPosition", RpcTarget.All,
            spawn.position.x, spawn.position.y, spawn.position.z,
            spawn.rotation.x, spawn.rotation.y, spawn.rotation.z, spawn.rotation.w);
    }
}