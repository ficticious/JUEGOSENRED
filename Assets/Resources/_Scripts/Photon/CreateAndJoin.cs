using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    [Header("Inputs")]
    public TMP_InputField inputCreate;
    public TMP_InputField inputJoin;
    public TextMeshProUGUI maxPlayersText;
    public Toggle isPublic;

    [Header("Room Options")]
    [Min(2)] public int _maxPlayers = 2; 
    public bool _isVisible;
    public bool _isOpen;

    private const int MIN_PLAYERS = 2;
    private const int MAX_PLAYERS = 4;

    public void CreateAndJoinRandomRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(inputCreate.text))
        {
            Debug.LogWarning("Cannot create room --- Need a room name");
        }
        else
        {
            PhotonNetwork.CreateRoom(inputCreate.text, new RoomOptions()
            {
                MaxPlayers = (byte)_maxPlayers,
                IsVisible = isPublic.isOn,
                IsOpen = isPublic.isOn,
            });
        }
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(inputJoin.text);
    }

    public void MorePlayers()
    {
        if (_maxPlayers < MAX_PLAYERS) _maxPlayers++;
        UpdateMaxPlayersText();
    }

    public void LessPlayers()
    {
        if (_maxPlayers > MIN_PLAYERS) _maxPlayers--;
        UpdateMaxPlayersText();
    }

    private void UpdateMaxPlayersText()
    {
        maxPlayersText.text = _maxPlayers.ToString();
    }
}
