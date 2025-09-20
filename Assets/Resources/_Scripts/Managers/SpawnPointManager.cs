using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnpointManager : MonoBehaviourPunCallbacks
{
    public static SpawnpointManager Instance;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RespawnPlayer(Player player)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No hay spawn points asignados!");
            return;
        }

        int rand = Random.Range(0, spawnPoints.Length);
        Transform spawn = spawnPoints[rand];

        // Instancia un nuevo player
        GameObject newPlayer = PhotonNetwork.Instantiate("PlayerPrefab", spawn.position, spawn.rotation, 0);

        // Resetear vida
        newPlayer.GetComponent<Health>().ResetHealth();
        newPlayer.GetComponent<PlayerSetup>().EnablePlayer();
    }
}