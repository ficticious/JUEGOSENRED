using System.Collections;
using Photon.Pun;
using UnityEngine;

public class GulagManager : MonoBehaviourPunCallbacks
{
    public static GulagManager Instance;

    [Header("Settings")]
    public Transform[] gulagSpawnPoints;
    public GameObject[] wallsToDisable;
    public GameObject[] targetObjects;
    public float loserRespawnDelay = 10f;

    public bool GulagActive = false;

    private PhotonView pv;

    // NUEVOS MÓDULOS
    private GulagQueueManager queue;
    private GulagTeleportService teleporter;
    private GulagEnvironmentController environment;

    private void Awake()
    {
        Instance = this;
        pv = GetComponent<PhotonView>();

        queue = new GulagQueueManager();
        teleporter = new GulagTeleportService();
        environment = new GulagEnvironmentController(wallsToDisable, targetObjects);
    }

    // --------------------- ENTRADA AL GULAG ---------------------

    public void RequestGulagEntry(int playerViewID)
    {
        pv.RPC("RPC_AddPlayer", RpcTarget.MasterClient, playerViewID);
    }

    [PunRPC]
    private void RPC_AddPlayer(int viewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        queue.AddPlayer(viewID);

        if (queue.HasOnePlayer())
        {
            pv.RPC("RPC_TeleportWaiting", RpcTarget.All, viewID, 0);
        }
        else if (queue.HasTwoPlayers())
        {
            StartDuel();
        }
    }

    // ----------------------- INICIO DEL DUELO -----------------------

    private void StartDuel()
    {
        GulagActive = true;

        pv.RPC("RPC_StartDuel", RpcTarget.All,
            queue.GetFirstPlayer(),
            queue.GetSecondPlayer());
    }

    [PunRPC]
    private void RPC_TeleportWaiting(int viewID, int spawn)
    {
        Health h = PhotonView.Find(viewID).GetComponent<Health>();
        teleporter.TeleportPlayer(h, gulagSpawnPoints[spawn]);
        h.ResetHealth();
        h.isDead = false;
        h.playerSetup.EnablePlayer();
        if (h.photonView.IsMine)
            h.playerSetup.EnableLocalCamera(true);
    }

    [PunRPC]
    private void RPC_StartDuel(int id1, int id2)
    {
        Health p1 = PhotonView.Find(id1).GetComponent<Health>();
        Health p2 = PhotonView.Find(id2).GetComponent<Health>();

        teleporter.TeleportPlayer(p1, gulagSpawnPoints[0]);
        teleporter.TeleportPlayer(p2, gulagSpawnPoints[1]);

        environment.EnableArena();

        p1.ResetHealth(); p2.ResetHealth();
        p1.isDead = p2.isDead = false;

        if (p1.photonView.IsMine)
        {
            p1.playerSetup.EnablePlayer();
            p1.playerSetup.EnableLocalCamera(true);
        }
        if (p2.photonView.IsMine)
        {
            p2.playerSetup.EnablePlayer();
            p2.playerSetup.EnableLocalCamera(true);
        }
    }

    // ----------------------- FINAL DEL DUELO -----------------------


    public void OnTargetHit(int shooterViewID)
    {
        if (!PhotonNetwork.IsMasterClient || !GulagActive) return;

        int loserID = queue.GetOpponent(shooterViewID);


        PhotonView winnerView = PhotonView.Find(shooterViewID);
        if (winnerView != null)
        {
            pv.RPC("RPC_NotifyWinner", winnerView.Owner);
        }

        if (loserID != -1)
        {
            PhotonView loserView = PhotonView.Find(loserID);
            if (loserView != null)
            {
                pv.RPC("RPC_NotifyLoser", loserView.Owner);
            }
        }


        pv.RPC("RPC_GulagEnded", RpcTarget.All);

        queue.Clear();
        GulagActive = false;
    }

    [PunRPC]
    private void RPC_NotifyWinner()
    {
        Health winner = GetLocalPlayerHealth();
        if (winner != null)
            StartCoroutine(winner.RespawnAfterGulag());
    }

    [PunRPC]
    private void RPC_NotifyLoser()
    {
        Health loser = GetLocalPlayerHealth();
        if (loser != null)
            StartCoroutine(RespawnLoser(loser));
    }

    [PunRPC]
    private void RPC_GulagEnded()
    {
        environment.DisableArena();
    }

    private Health GetLocalPlayerHealth()
    {

        foreach (var player in FindObjectsOfType<Health>())
        {
            if (player.photonView.IsMine)
                return player;
        }
        return null;
    }

    private IEnumerator RespawnLoser(Health h)
    {
        yield return new WaitForSeconds(loserRespawnDelay);
        yield return h.StartCoroutine(h.RespawnAfterGulag());
    }


    //public void OnTargetHit(int shooterViewID)
    //{
    //    if (!PhotonNetwork.IsMasterClient || !GulagActive) return;

    //    int loserID = queue.GetOpponent(shooterViewID);

    //    pv.RPC("RPC_DuelFinished", RpcTarget.All, shooterViewID, loserID);
    //    queue.Clear();
    //    GulagActive = false;
    //}

    //[PunRPC]
    //private void RPC_DuelFinished(int winnerID, int loserID)
    //{
    //    environment.DisableArena();

    //    Health winner = PhotonView.Find(winnerID).GetComponent<Health>();
    //    if (winner.photonView.IsMine)
    //        StartCoroutine(winner.RespawnAfterGulag());

    //    if (loserID != -1)
    //    {
    //        Health loser = PhotonView.Find(loserID).GetComponent<Health>();
    //        if (loser != null && loser.photonView.IsMine)
    //            StartCoroutine(RespawnLoser(loser));
    //    }
    //}

    //private IEnumerator RespawnLoser(Health h)
    //{
    //    yield return new WaitForSeconds(loserRespawnDelay);
    //    yield return h.StartCoroutine(h.RespawnAfterGulag());
    //}
}
