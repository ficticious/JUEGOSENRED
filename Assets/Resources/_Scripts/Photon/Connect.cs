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

    //public SpawnController newPlayer;
    
    [Space]
    [Header("Player")]
    public GameObject mainCamera;
    public GameObject player;

    private TMP_InputField inputNickname;
    private string nickname = "player";
    private int numberOfPlayer;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
    }

    // -------------------  PHOTON  -----------------------------------------------------------
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

        GameObject newPlayer = SpawnManager.instance.SpawnPlayer();
        newPlayer.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, PhotonNetwork.NickName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

// ------------------------------------------------------------------------------

    public void ChangeNickname(string _name)
    {
        //nickname = _name;

        //PhotonNetwork.NickName = _name;
    }

    private void EmptyNickname()
    {
        if (inputNickname == null || inputNickname.text.Length == 0)
        {
            numberOfPlayer = PhotonNetwork.CountOfPlayersInRooms + 1;
            nickname = nickname + numberOfPlayer.ToString();
        }
    }


    public void LocalPlayer()
    {
        //player.GetComponent<Health>().isLocalPlayer = true;
    }
}
