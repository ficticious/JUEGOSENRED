using Photon.Pun;
using System.IO;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public static SpawnManager instance;

    public GameObject player;
    public float respawnTime = 30f;
    public Transform[] spawnPoints;

    [Header("GAMEOBJECTS")]
    public GameObject canvasLoading;
    public GameObject canvasLobby;

    private GameObject mainCamera;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        mainCamera = FindObjectOfType<Camera>().gameObject;
    }

    private void Start()
    {
        canvasLobby.SetActive(false);
    }

    public override void OnJoinedLobby()
    {
        if (canvasLoading != null)
            canvasLoading.SetActive(false);

        canvasLobby.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        canvasLobby.SetActive(false);
        mainCamera.SetActive(false);
    }

    public GameObject SpawnPlayer()
    {
        Transform spawn = SpawnPointManager.Instance.GetSafeSpawnPoint(
            SpawnPointManager.Instance.minDistanceBetweenPlayers
        );

        // Rotación corregida (180 grados opcional)
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
