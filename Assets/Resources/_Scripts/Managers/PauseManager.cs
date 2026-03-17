using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject chat;

    private bool isPaused = false;

    private PlayerInputBlocker localPlayer;

    private void Start()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();

        //localPlayer = FindLocalPlayer();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
            return;

        if (!PhotonNetwork.LocalPlayer.IsLocal)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (localPlayer == null)
                localPlayer = FindLocalPlayer();

            if (isPaused)
                ResumeGame();
            else
                PauseGame();

            chat.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            chat.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void PauseGame()
    {
        //localPlayer = FindLocalPlayer();


        isPaused = true;

        pauseMenuUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //DisableLocalPlayerControls(true);

        if (localPlayer != null)
            localPlayer.SetBlocked(true);

    }

    public void ResumeGame()
    {
        isPaused = false;

        pauseMenuUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //DisableLocalPlayerControls(false);

        if (localPlayer != null)
            localPlayer.SetBlocked(false);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        pauseMenuUI.SetActive(false);
        isPaused = false;

        cam.SetActive(true);
    }

    private PlayerInputBlocker FindLocalPlayer()
    {
        //Debug.Log("aaaaa");

        PlayerInputBlocker[] players = FindObjectsOfType<PlayerInputBlocker>();

        foreach (var p in players)
        {
            if (p.photonView != null && p.photonView.IsMine)
            {
                //Debug.Log(p);
                return p;
            }
        }

        return null;
    }
}
