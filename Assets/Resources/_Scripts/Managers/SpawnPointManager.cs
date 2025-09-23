using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnPointManager : MonoBehaviourPunCallbacks
{
    public static SpawnPointManager Instance;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public Transform GetRandomSpawnPoint()
    {
        int rand = Random.Range(0, spawnPoints.Length);
        return spawnPoints[rand];
    }
}