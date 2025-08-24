using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MyPhotonPlayer : MonoBehaviour
{
    PhotonView myPV;
    GameObject myPlayerAvatar;
    private PlayerSetup playerSetup;

    Player[] allPlayers;
    int myNumberInRoom;

    private void Awake()
    {
        myPV = GetComponent<PhotonView>();
        playerSetup = GetComponent<PlayerSetup>();
    }

    private void Start()
    {
        if (myPV.IsMine)
        {
            playerSetup.IsLocalPlayer();
            Debug.Log("Jugador local instanciado");
        }
        else
        {
            playerSetup.IsRemotePlayer();
        }
    }
}
