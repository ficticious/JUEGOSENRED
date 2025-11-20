using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GulagManager : MonoBehaviourPunCallbacks
{
    public static GulagManager Instance;

    [Header("Gulag 1 Settings")]
    public Transform[] gulag1SpawnPoints;
    public GameObject[] gulag1WallsToDisable;
    public GameObject gulag1TargetObject;

    [Header("Gulag 2 Settings")]
    public Transform[] gulag2SpawnPoints;
    public GameObject[] gulag2WallsToDisable;
    public GameObject gulag2TargetObject;

    [Header("General Settings")]
    public float loserRespawnDelay = 10f;

    private bool gulag1Active = false;
    private bool gulag2Active = false;

    private List<int> waitingPlayerViewIDs = new List<int>();
    private List<int> gulag1Players = new List<int>();
    private List<int> gulag2Players = new List<int>();

    private int gulag1WaitingPlayer = -1;
    private int gulag2WaitingPlayer = -1;

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

        Debug.Log($"[MASTER] Jugador {playerViewID} agregado a la cola del gulag.");

        waitingPlayerViewIDs.Add(playerViewID);

        Debug.Log($"[MASTER] Total esperando: {waitingPlayerViewIDs.Count}");

        ProcessGulagQueue();
    }

    private void ProcessGulagQueue()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log($"[MASTER] ProcessGulagQueue - Esperando: {waitingPlayerViewIDs.Count}, Gulag1 activo: {gulag1Active}, Gulag2 activo: {gulag2Active}");

        if (gulag1WaitingPlayer != -1 && waitingPlayerViewIDs.Count > 0 && !gulag1Active)
        {
            int player2 = waitingPlayerViewIDs[0];
            waitingPlayerViewIDs.RemoveAt(0);

            gulag1Players.Clear();
            gulag1Players.Add(gulag1WaitingPlayer);
            gulag1Players.Add(player2);

            Debug.Log($"[MASTER] Emparejando en Gulag 1: {gulag1WaitingPlayer} vs {player2}");
            StartGulag(1, gulag1WaitingPlayer, player2);
            gulag1WaitingPlayer = -1;
            return;
        }

        if (gulag2WaitingPlayer != -1 && waitingPlayerViewIDs.Count > 0 && !gulag2Active)
        {
            int player2 = waitingPlayerViewIDs[0];
            waitingPlayerViewIDs.RemoveAt(0);

            gulag2Players.Clear();
            gulag2Players.Add(gulag2WaitingPlayer);
            gulag2Players.Add(player2);

            Debug.Log($"[MASTER] Emparejando en Gulag 2: {gulag2WaitingPlayer} vs {player2}");
            StartGulag(2, gulag2WaitingPlayer, player2);
            gulag2WaitingPlayer = -1;
            return;
        }

        if (!gulag1Active && waitingPlayerViewIDs.Count >= 2)
        {
            int player1 = waitingPlayerViewIDs[0];
            int player2 = waitingPlayerViewIDs[1];

            waitingPlayerViewIDs.RemoveAt(0);
            waitingPlayerViewIDs.RemoveAt(0);

            gulag1Players.Clear();
            gulag1Players.Add(player1);
            gulag1Players.Add(player2);

            Debug.Log($"[MASTER] Iniciando Gulag 1 con jugadores {player1} y {player2}");
            StartGulag(1, player1, player2);
            return;
        }

        if (!gulag2Active && waitingPlayerViewIDs.Count >= 2)
        {
            int player1 = waitingPlayerViewIDs[0];
            int player2 = waitingPlayerViewIDs[1];

            waitingPlayerViewIDs.RemoveAt(0);
            waitingPlayerViewIDs.RemoveAt(0);

            gulag2Players.Clear();
            gulag2Players.Add(player1);
            gulag2Players.Add(player2);

            Debug.Log($"[MASTER] Iniciando Gulag 2 con jugadores {player1} y {player2}");
            StartGulag(2, player1, player2);
            return;
        }

        if (waitingPlayerViewIDs.Count == 1)
        {
            int playerID = waitingPlayerViewIDs[0];
            waitingPlayerViewIDs.RemoveAt(0);

            if (!gulag1Active && gulag1WaitingPlayer == -1)
            {
                gulag1WaitingPlayer = playerID;
                Debug.Log($"[MASTER] Jugador {playerID} esperando en Gulag 1");
                pv.RPC("RPC_TeleportToWaiting", RpcTarget.All, playerID, 1, 0);
            }
            else if (!gulag2Active && gulag2WaitingPlayer == -1)
            {
                gulag2WaitingPlayer = playerID;
                Debug.Log($"[MASTER] Jugador {playerID} esperando en Gulag 2");
                pv.RPC("RPC_TeleportToWaiting", RpcTarget.All, playerID, 2, 0);
            }
            else
            {
                waitingPlayerViewIDs.Add(playerID);
                Debug.Log($"[MASTER] Jugador {playerID} vuelve a la cola - Ambos gulags ocupados");
            }
        }
    }

    private void StartGulag(int gulagNumber, int player1ViewID, int player2ViewID)
    {
        if (gulagNumber == 1)
            gulag1Active = true;
        else
            gulag2Active = true;

        pv.RPC("RPC_StartDuel", RpcTarget.All, gulagNumber, player1ViewID, player2ViewID);
    }

    [PunRPC]
    private void RPC_TeleportToWaiting(int viewID, int gulagNumber, int spawnIndex)
    {
        Debug.Log($"[RPC] RPC_TeleportToWaiting - ViewID: {viewID}, Gulag: {gulagNumber}, Spawn: {spawnIndex}");

        Health player = PhotonView.Find(viewID).GetComponent<Health>();
        if (player == null)
        {
            Debug.LogError($"[ERROR] No se encontró jugador con ViewID {viewID}");
            return;
        }

        Transform[] spawnPoints = gulagNumber == 1 ? gulag1SpawnPoints : gulag2SpawnPoints;

        TeleportPlayer(player, spawnPoints[spawnIndex]);

        player.ResetHealth();
        player.isDead = false;
        player.playerSetup.EnablePlayer();

        if (player.photonView.IsMine)
        {
            Debug.Log($"[LOCAL] Es mi jugador, habilitando cámara en Gulag {gulagNumber}");
            player.playerSetup.EnableLocalCamera(true);
        }
    }

    [PunRPC]
    private void RPC_StartDuel(int gulagNumber, int viewID1, int viewID2)
    {
        Debug.Log($"[RPC] RPC_StartDuel - Gulag {gulagNumber}, ViewID1: {viewID1}, ViewID2: {viewID2}");

        Health player1 = PhotonView.Find(viewID1).GetComponent<Health>();
        Health player2 = PhotonView.Find(viewID2).GetComponent<Health>();

        if (player1 == null || player2 == null)
        {
            Debug.LogError($"[ERROR] No se encontraron los jugadores");
            return;
        }

        Transform[] spawnPoints;
        GameObject[] walls;
        GameObject target;

        if (gulagNumber == 1)
        {
            spawnPoints = gulag1SpawnPoints;
            walls = gulag1WallsToDisable;
            target = gulag1TargetObject;
        }
        else
        {
            spawnPoints = gulag2SpawnPoints;
            walls = gulag2WallsToDisable;
            target = gulag2TargetObject;
        }

        Debug.Log($"[RPC] Teletransportando jugadores al Gulag {gulagNumber}");
        TeleportPlayer(player1, spawnPoints[0]);
        TeleportPlayer(player2, spawnPoints[1]);

        foreach (GameObject wall in walls)
        {
            wall.SetActive(false);
        }

        if (target != null)
        {
            target.SetActive(true);
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

    public void OnTargetHit(int gulagNumber, int shooterViewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log($"[MASTER] Target del Gulag {gulagNumber} golpeado por ViewID: {shooterViewID}");

        Health winner = PhotonView.Find(shooterViewID).GetComponent<Health>();
        if (winner == null) return;

        List<int> currentGulagPlayers = gulagNumber == 1 ? gulag1Players : gulag2Players;
        Health loser = null;

        foreach (int viewID in currentGulagPlayers)
        {
            if (viewID != shooterViewID)
            {
                loser = PhotonView.Find(viewID).GetComponent<Health>();
                break;
            }
        }

        pv.RPC("RPC_DuelFinished", RpcTarget.All, gulagNumber,
            winner.photonView.ViewID,
            loser != null ? loser.photonView.ViewID : -1);

        if (gulagNumber == 1)
        {
            gulag1Players.Clear();
            gulag1Active = false;
        }
        else
        {
            gulag2Players.Clear();
            gulag2Active = false;
        }

        ProcessGulagQueue();
    }

    [PunRPC]
    private void RPC_DuelFinished(int gulagNumber, int winnerViewID, int loserViewID)
    {
        Debug.Log($"[RPC] Duelo del Gulag {gulagNumber} finalizado - Ganador: {winnerViewID}, Perdedor: {loserViewID}");

        Health winner = PhotonView.Find(winnerViewID).GetComponent<Health>();
        if (winner == null)
        {
            Debug.LogError($"[ERROR] No se encontró el ganador con ViewID {winnerViewID}");
            return;
        }

        GameObject[] walls = gulagNumber == 1 ? gulag1WallsToDisable : gulag2WallsToDisable;
        GameObject target = gulagNumber == 1 ? gulag1TargetObject : gulag2TargetObject;

        foreach (GameObject wall in walls)
        {
            wall.SetActive(true);
        }

        if (target != null)
        {
            target.SetActive(false);
        }

        if (winner.photonView.IsMine)
        {
            Debug.Log($"[LOCAL] Soy el ganador ViewID {winnerViewID}, respawneando al mapa");
            winner.StartCoroutine(winner.RespawnAfterGulag());
        }

        if (loserViewID != -1)
        {
            Health loser = PhotonView.Find(loserViewID).GetComponent<Health>();
            if (loser == null)
            {
                Debug.LogError($"[ERROR] No se encontró el perdedor con ViewID {loserViewID}");
                return;
            }

            if (loser.photonView.IsMine)
            {
                Debug.Log($"[LOCAL] Soy el perdedor ViewID {loserViewID}, esperando {loserRespawnDelay} segundos");
                loser.StartCoroutine(RespawnLoserAfterDelay(loser));
            }
        }
    }

    private IEnumerator RespawnLoserAfterDelay(Health loser)
    {
        Debug.Log($"[COROUTINE] Esperando {loserRespawnDelay} segundos para respawnear perdedor");

        yield return new WaitForSeconds(loserRespawnDelay);

        if (loser != null && loser.photonView.IsMine)
        {
            Debug.Log($"[COROUTINE] Respawneando perdedor {loser.photonView.ViewID}");
            loser.StartCoroutine(loser.RespawnAfterGulag());
        }
        else
        {
            Debug.LogError($"[COROUTINE ERROR] Perdedor es null o no es mío");
        }
    }

    private void TeleportPlayer(Health p, Transform spawn)
    {
        if (spawn == null)
        {
            Debug.LogError("[ERROR] Spawn point es NULL!");
            return;
        }

        p.photonView.RPC("SetRespawnPosition", RpcTarget.All,
            spawn.position.x, spawn.position.y, spawn.position.z,
            spawn.rotation.x, spawn.rotation.y, spawn.rotation.z, spawn.rotation.w);
    }
}
