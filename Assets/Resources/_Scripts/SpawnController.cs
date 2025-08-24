using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine.SceneManagement;


public class SpawnController : MonoBehaviourPunCallbacks
{
    public static SpawnController instance;

    public GameObject player;
    public string nickname;
    public Transform[] spawnPoints;

    private void Awake()
    {
        instance = this;
    }

    //public override void OnJoinedRoom()
    //{
    //    base.OnJoinedRoom();
    //    Debug.Log("SPAWNCONTROLLER onjoined");
    //    SceneManager.LoadScene("Game");
    //    SpawnPlayer();
    //}

    public GameObject SpawnPlayer()
    {
        int spawnIndex = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoints[spawnIndex].position, Quaternion.identity);
        return _player;
        //_player = GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
    }
}
