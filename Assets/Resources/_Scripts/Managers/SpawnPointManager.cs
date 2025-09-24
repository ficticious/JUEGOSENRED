using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class SpawnPointManager : MonoBehaviourPunCallbacks
{
    public static SpawnPointManager Instance;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public Transform GetSafeSpawnPoint(float minDistance = 5f)
    {
        List<Transform> safeSpawns = new List<Transform>();

        foreach (var spawn in spawnPoints)
        {
            bool isSafe = true;

            // revisar distancia con todos los jugadores vivos
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                Health h = player.GetComponent<Health>();
                if (h != null && h.health > 0) // jugador vivo
                {
                    float dist = Vector3.Distance(spawn.position, player.transform.position);
                    if (dist < minDistance)
                    {
                        isSafe = false;
                        break;
                    }
                }
            }

            if (isSafe)
                safeSpawns.Add(spawn);
        }

        // si no encontró ninguno "seguro", usar cualquiera
        if (safeSpawns.Count == 0)
            return spawnPoints[Random.Range(0, spawnPoints.Length)];

        return safeSpawns[Random.Range(0, safeSpawns.Count)];
    }
}