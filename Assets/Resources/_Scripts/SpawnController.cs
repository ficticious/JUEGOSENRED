using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using UnityEngine.SceneManagement;


public class SpawnController : MonoBehaviour
{
    public static SpawnController instance;

    public GameObject player;
    public string nickname;
    public Transform[] spawnPoints;

    private void Awake()
    {
        instance = this;
    }

    public GameObject SpawnPlayer()
    {
        int spawnIndex = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;
        Transform spawn = spawnPoints[spawnIndex];

        Quaternion spawnRotation = Quaternion.Euler(
            spawn.rotation.eulerAngles.x,
            spawn.rotation.eulerAngles.y + 180f,
            spawn.rotation.eulerAngles.z
        );

        GameObject _player = PhotonNetwork.Instantiate(
            Path.Combine("_Prefabs", "Player"),
            spawn.position,
            spawnRotation
        );
        return _player;
    }
}
