using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPun
{
    public CameraMove cameraScript;
    public Movement movementScript;
    public GameObject playerCamera;
    public string nickname;
    public TextMeshPro nameTag;

    public GameObject playerModel;

    [PunRPC]
    public void SetNickname(string _name)
    {
        nickname = _name;
        nameTag.text = nickname;
    }

    public void IsLocalPlayer()
    {
        cameraScript.enabled = true;
        playerCamera.SetActive(true);
        movementScript.enabled = true;
    }

    public void IsRemotePlayer()
    {
        movementScript.enabled = false;
        cameraScript.enabled = false;
        playerCamera.SetActive(false);

        Debug.Log("Remote Player");
    }

    public void DisablePlayer()
    {
        movementScript.enabled = false;

        
        if (!photonView.IsMine)
        {
            cameraScript.enabled = false;
            playerCamera.SetActive(false);
        }

        if (playerModel != null)
            playerModel.SetActive(false);
    }


    public void EnablePlayer()
    {
        movementScript.enabled = true;

        if (playerModel != null)
            playerModel.SetActive(true);

        
        if (photonView.IsMine)
        {
            cameraScript.enabled = true;
            playerCamera.SetActive(true);
        }
    }


}
