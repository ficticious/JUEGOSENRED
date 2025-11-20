using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GulagMatchManager : MonoBehaviourPun
{
    public static GulagMatchManager Instance;

    [Header("Match Points")]
    public Transform p1Spawn;
    public Transform p2Spawn;

    [Header("Objects")]
    public GameObject gulagWalls;
    public GulagMinigame minigame;

    private void Awake()
    {
        Instance = this;
    }

    public void StartMatch(Health p1, Health p2)
    {
        photonView.RPC("RPC_StartMatch", RpcTarget.All,
            p1.photonView.ViewID, p2.photonView.ViewID);
    }

    [PunRPC]
    private void RPC_StartMatch(int p1ID, int p2ID)
    {
        Health p1 = PhotonView.Find(p1ID).GetComponent<Health>();
        Health p2 = PhotonView.Find(p2ID).GetComponent<Health>();

        TeleportPlayer(p1.transform, p1Spawn);
        TeleportPlayer(p2.transform, p2Spawn);

        gulagWalls.SetActive(false);

        minigame.StartMinigame(p1, p2);
    }

    private void TeleportPlayer(Transform t, Transform target)
    {
        CharacterController cc = t.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        t.position = target.position;
        t.rotation = target.rotation;

        if (cc) cc.enabled = true;
    }

    public void FinishMatch(Health winner, Health loser)
    {
        gulagWalls.SetActive(true);
        GulagManager.Instance.EndMatch(winner, loser);
    }
}
