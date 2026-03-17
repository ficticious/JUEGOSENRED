using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager Instance { get; private set; }

    [Header("Spawn Settings")]
    public float minDistanceBetweenPlayers = 10f;

    [Header("Debug Settings")]
    public bool showDebugGizmos = true;
    public Color safeColor = Color.green;
    public Color unsafeColor = Color.red;
    public float gizmoRadius = 1.5f;

    private Transform[] spawnPoints;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Usamos Linq para filtrar los nulos rápidamente
        spawnPoints = SpawnManager.instance.spawnPoints.Where(sp => sp != null).ToArray();

        if (spawnPoints.Length == 0)
            Debug.LogWarning("No hay Spawnpoints asignados.");
    }

    public Transform GetSafeSpawnPoint(float minDistance = 10f)
    {
        List<Transform> safeSpawns = new List<Transform>();

        foreach (Transform spawn in spawnPoints)
        {
            bool isSafe = true;

            // OPTIMIZACIÓN: Usamos la lista estática en lugar de FindGameObjectsWithTag
            foreach (Health playerHealth in Health.AllActivePlayers)
            {
                if (playerHealth.IsDead) continue;

                float distance = Vector3.Distance(spawn.position, playerHealth.transform.position);
                if (distance < minDistance)
                {
                    isSafe = false;
                    break;
                }
            }

            if (isSafe) safeSpawns.Add(spawn);
        }

        if (safeSpawns.Count == 0) return GetFarthestSpawnPoint();

        return safeSpawns[Random.Range(0, safeSpawns.Count)];
    }

    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return null;
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    private Transform GetFarthestSpawnPoint()
    {
        Transform farthestSpawn = spawnPoints[0];
        float maxDistance = -1f;

        foreach (Transform spawn in spawnPoints)
        {
            float totalDistance = 0f;
            int playerCount = 0;

            foreach (Health playerHealth in Health.AllActivePlayers)
            {
                if (!playerHealth.IsDead)
                {
                    totalDistance += Vector3.Distance(spawn.position, playerHealth.transform.position);
                    playerCount++;
                }
            }

            float averageDistance = playerCount > 0 ? totalDistance / playerCount : float.MaxValue;

            if (averageDistance > maxDistance)
            {
                maxDistance = averageDistance;
                farthestSpawn = spawn;
            }
        }

        return farthestSpawn;
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos || spawnPoints == null) return;

        foreach (Transform spawn in spawnPoints)
        {
            bool isSafe = true;

            if (Application.isPlaying)
            {
                foreach (Health playerHealth in Health.AllActivePlayers)
                {
                    if (!playerHealth.IsDead && Vector3.Distance(spawn.position, playerHealth.transform.position) < minDistanceBetweenPlayers)
                    {
                        isSafe = false;
                        break;
                    }
                }
            }

            Gizmos.color = isSafe ? safeColor : unsafeColor;
            Gizmos.DrawSphere(spawn.position, gizmoRadius);
            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.2f);
            Gizmos.DrawWireSphere(spawn.position, minDistanceBetweenPlayers);
        }
    }
}
