using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPun
{
    public CameraMove cameraScript;
    public Movement movementScript;
    public GameObject camera;
    public string nickname;
    public TextMeshPro nameTag;

    [Header("Stats")]
    public int kills = 0;
    public int deaths = 0;

    [PunRPC]
    public void SetNickname(string _name)
    {
        nickname = _name;
        if (nameTag != null)
            nameTag.text = nickname;
    }

    public void IsLocalPlayer()
    {
        cameraScript.enabled = true;
        camera.SetActive(true);
        movementScript.enabled = true;
    }

    public void IsRemotePlayer()
    {
        movementScript.enabled = false;
        cameraScript.enabled = false;
        camera.SetActive(false);
        Debug.Log("Remote Player");
    }

 
}