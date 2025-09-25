using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Connect : MonoBehaviourPunCallbacks
{
    public static Connect instance;
    
    [Space]
    [Header("Player")]
    public GameObject mainCamera;
    public GameObject player;
    public TMP_InputField inputNickname;
    private string nickname = "player";
    private int numberOfPlayer;

    [HideInInspector]
    public int kills = 0;
    [HideInInspector]
    public int deaths = 0;

    private void Awake()
    {
        instance = this;
    }
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

        LocalPlayer();

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();


        

        GameObject newPlayer = SpawnController.instance.SpawnPlayer();
        EmptyNickname();
        newPlayer.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        LocalPlayer();

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

    public void SetHashes()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
            hash["kills"] = kills;
            hash["deaths"] = deaths;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {
           
        }
    }

    public void LocalPlayer()
    {
        player.GetComponent<Health>().isLocalPlayer = true;
    }

}
