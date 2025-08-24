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
    [Min(1)] public int _maxPlayers;
    public bool _isVisible;
    public bool _isOpen;

    public void CreateAndJoinRandomRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public void CreateRoom()
    {
        if (inputCreate.text == "" || inputCreate.text == null)
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
        if (_maxPlayers < 10) _maxPlayers++;
        UpdateMaxPlayersText();
    }

    public void LessPlayers()
    {
        if (_maxPlayers > 1) _maxPlayers--;
        UpdateMaxPlayersText();
    }

    private void UpdateMaxPlayersText()
    {
        maxPlayersText.text = _maxPlayers.ToString();
    }
}
