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

    //[HideInInspector]
    //public int kills = 0;
    //public int deaths = 0;

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

        //GameObject newPlayer = SpawnController.instance.SpawnPlayer();
        //newPlayer.SpawnPlayer();
        GameObject newPlayer = SpawnManager.instance.SpawnPlayer();
        EmptyNickname();
        newPlayer.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        LocalPlayer();

        PhotonNetwork.LocalPlayer.NickName = nickname;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

// ------------------------------------------------------------------------------

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

    //public void SetHashes()
    //{
    //    try
    //    {
    //        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
    //        hash["kills"] = kills;
    //        hash["deaths"] = deaths;

    //        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    //    }
    //    catch { }
    //}

    public void LocalPlayer()
    {
        player.GetComponent<Health>().isLocalPlayer = true;
    }

}
