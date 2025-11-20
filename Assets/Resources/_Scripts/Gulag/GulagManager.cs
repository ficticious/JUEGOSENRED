using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GulagManager : MonoBehaviourPunCallbacks
{
    public static GulagManager Instance;

    [Header("Gulag Settings")]
    public Transform waitingRoomPoint;
    public float waitRespawnTime = 8f;

    private List<Health> queue = new List<Health>();
    private bool waitingForSecondPlayer = false;

    private void Awake()
    {
        Instance = this;
    }

    // --------------------------- PUBLIC ENTRY ---------------------------

    public void RequestGulagEntry(int viewID)
    {
        photonView.RPC("RPC_AddPlayerToQueue", RpcTarget.MasterClient, viewID);
    }

    // --------------------------- RPC ---------------------------

    [PunRPC]
    private void RPC_AddPlayerToQueue(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);
        if (pv == null) return;

        Health player = pv.GetComponent<Health>();
        if (player == null || queue.Contains(player)) return;

        queue.Add(player);

        if (queue.Count == 1)
        {
            photonView.RPC("RPC_TeleportToWaitingRoom", RpcTarget.All, viewID);
            StartCoroutine(WaitingRespawnRoutine(player));
        }
        else if (queue.Count == 2)
        {
            StopAllCoroutines();
            StartMatch();
        }
    }

    // ------------------------- TELEPORT ---------------------------

    [PunRPC]
    private void RPC_TeleportToWaitingRoom(int viewID)
    {
        PhotonView pv = PhotonView.Find(viewID);
        if (pv == null) return;

        Transform t = pv.transform;
        CharacterController cc = t.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        t.position = waitingRoomPoint.position;
        t.rotation = waitingRoomPoint.rotation;

        if (cc) cc.enabled = true;
    }

    // ------------------------- WAITING LOGIC --------------------------

    private IEnumerator WaitingRespawnRoutine(Health player)
    {
        waitingForSecondPlayer = true;
        float timer = waitRespawnTime;

        while (timer > 0f)
        {
            yield return new WaitForSeconds(1f);
            timer--;

            if (queue.Count == 2)
                yield break;
        }

        queue.Remove(player);
        player.StartCoroutine(player.RespawnAfterGulag());
    }

    // -------------------------------- MATCH ---------------------------------

    private void StartMatch()
    {
        if (queue.Count < 2) return;

        Health p1 = queue[0];
        Health p2 = queue[1];

        GulagMatchManager.Instance.StartMatch(p1, p2);
    }

    public void EndMatch(Health winner, Health loser)
    {
        queue.Clear();

        winner.StartCoroutine(winner.RespawnAfterGulag());
        loser.StartCoroutine(loser.RespawnAfterGulag());
    }
}