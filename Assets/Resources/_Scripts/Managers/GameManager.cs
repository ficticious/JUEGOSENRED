using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    public GameObject losePanel, winPanel, buttonLobby;

    [Header("KD Count")]
    public int kills = 0;
    public int deaths = 0;
    [SerializeField] private int killsToWin;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public void GoToLobby()
    {
        PhotonNetwork.Disconnect();
        kills = 0;
        deaths = 0;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("kills"))
        {
            int kills = (int)targetPlayer.CustomProperties["kills"];

            if (kills >= killsToWin)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                buttonLobby.SetActive(true);

                if (targetPlayer == PhotonNetwork.LocalPlayer)
                {
                    //FindObjectOfType<WinScreen>().ShowWinPanel();
                    winPanel.SetActive(true);
                    StopAllPlayers();
                }
                else
                {
                    //FindObjectOfType<GameOverScreen>().ShowLosePanel();
                    //ShowPanel();
                    losePanel.SetActive(true);
                    StopAllPlayers();
                }
            }
        }
    }

    public void StopAllPlayers()
    {
        var players = FindObjectsOfType<Movement>();
        var cameraPlayers = FindObjectsOfType<CameraMove>();

        foreach (var p in players)
        {
            p.enabled = false;
        }
        foreach (var d in cameraPlayers)
        {
            d.enabled = false;
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
        catch { }
    }
}
