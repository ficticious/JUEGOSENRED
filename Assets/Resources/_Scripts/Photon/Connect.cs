using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using TMPro;

public class Connect : MonoBehaviourPunCallbacks
{
    [Space]
    [Header("Player")]
    public GameObject mainCamera;
    public GameObject player;
    public TMP_InputField inputNickname;
    private string nickname = "player";
    private int numberOfPlayer;
    

    void Start()
    {
        if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
    }

    public void ConnectToMasterWithButton()
    {
        if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        GameObject newPlayer = SpawnController.instance.SpawnPlayer();
        EmptyNickname();
        newPlayer.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        player.GetComponent<Health>().isLocalPlayer = true;

        PhotonNetwork.LocalPlayer.NickName = nickname;
    }

    public void ChangeNickname(string _name)
    {
        nickname = _name;
    }

    private void EmptyNickname()
    {
        if (inputNickname == null || inputNickname.text.Length == 0)
        {
            numberOfPlayer = PhotonNetwork.CountOfPlayersInRooms + 1;
            nickname = nickname + numberOfPlayer.ToString();
        }
    }
}
