using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public static SpawnManager instance;

    public GameObject player;
    public string nickname;
    public float respawnTime = 0f;
    public Transform[] spawnPoints;

    [Header("GAMEOBJECTS")]
    [Space(5)]
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
        base.OnJoinedLobby();
        if (canvasLoading == null) return;
        canvasLoading.SetActive(false);
        canvasLobby.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        canvasLobby.SetActive(false);
        mainCamera.SetActive(false);
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


    //public IEnumerator RespawnCoroutine()
    //{
    //    yield return new WaitForSeconds(respawnTime);
    //    if (!photonView.IsMine) yield break;
    //    RespawnPlayer();
    //}

    //public void RespawnPlayer()
    //{
    //    if (!photonView.IsMine) return;
    //    Debug.Log("Holaaaa");

    //    Transform spawnPoint = SpawnPointManager.Instance.GetSafeSpawnPoint(10f);

    //    if (spawnPoint == null) spawnPoint = SpawnPointManager.Instance.GetRandomSpawnPoint();

    //    else
    //    {
    //        photonView.RPC("SetRespawnPosition", RpcTarget.All,
    //                      spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z,
    //                      spawnPoint.rotation.x, spawnPoint.rotation.y, spawnPoint.rotation.z, spawnPoint.rotation.w);
    //    }

    //    photonView.RPC("CompleteRespawn", RpcTarget.All);
    //}

    //[PunRPC]
    //public void SetRespawnPosition(float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float rotW)
    //{
    //    transform.position = new Vector3(posX, posY, posZ);
    //    transform.rotation = new Quaternion(rotX, rotY, rotZ, rotW);
    //}
}
