using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    [Header("GAMEOBJECTS")]
    [Space(5)]
    public GameObject canvasLoading;
    public GameObject canvasLobby;
    public GameObject mainCamera;

    private void Start()
    {
        canvasLobby.SetActive(false);
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        canvasLoading.SetActive(false);
        canvasLobby.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        canvasLobby.SetActive(false);
        mainCamera.SetActive(false);
    }
}
