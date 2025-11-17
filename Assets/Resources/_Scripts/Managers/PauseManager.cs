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
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
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

    //private void DisableLocalPlayerControls(bool disabled)
    //{
    //    Movement movement = FindObjectOfType<Movement>();
    //    if (movement != null && movement.photonView.IsMine)
    //    {
    //        movement.enabled = !disabled;
    //        movement.rb.velocity = Vector3.zero;
    //    }

    //    CameraMove cm = FindObjectOfType<CameraMove>();
    //    if (cm != null && movement.photonView.IsMine)
    //        cm.enabled = !disabled;

    //    //Shooting shooting = FindObjectOfType<Shooting>();
    //    //if (shooting != null && shooting.photonView.IsMine)
    //    //    shooting.enabled = !disabled;
    //}

    private PlayerInputBlocker FindLocalPlayer()
    {
        Debug.Log(localPlayer.name);

        PlayerInputBlocker[] players = FindObjectsOfType<PlayerInputBlocker>();

        foreach (var p in players)
        {
            if (p.photonView != null && p.photonView.IsMine)
                return p;
        }

        return null;
    }
}
