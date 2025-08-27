using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using TMPro;

public class Connect : MonoBehaviourPunCallbacks
{
    [Header("Player")]
    public GameObject mainCamera;
    public GameObject player;
    public TMP_InputField inputNickname;

    private string nickname = "Player";
    private int numberOfPlayer;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    public void ConnectToMasterWithButton()
    {
        
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();

        
        if (inputNickname != null && !string.IsNullOrEmpty(inputNickname.text))
        {
            nickname = inputNickname.text;
        }
        else
        {
            numberOfPlayer = PhotonNetwork.CountOfPlayersInRooms + 1;
            nickname = "Player" + numberOfPlayer;
        }

       
        PhotonNetwork.NickName = nickname;
        Debug.Log("Nick asignado: " + nickname);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedRoom()
    {
        
        GameObject newPlayer = SpawnController.instance.SpawnPlayer();

        
        newPlayer.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
    }

    public void ChangeNickname(string _name)
    {
        nickname = _name;
        PhotonNetwork.NickName = nickname;
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