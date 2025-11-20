using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class SpawnPointManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static SpawnPointManager Instance;

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
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        spawnPoints = SpawnManager.instance.spawnPoints;
        ValidateSpawnPoints();

        if (spawnPoints == null || spawnPoints.Length == 0)
            Debug.LogWarning("No hay Spawnpoints asignados.");
    }

    private void ValidateSpawnPoints()
    {
        List<Transform> validSpawns = new List<Transform>();
        foreach (Transform spawn in spawnPoints)
        {
            if (spawn != null)
                validSpawns.Add(spawn);
        }
        spawnPoints = validSpawns.ToArray();
    }

    public Transform GetSafeSpawnPoint(float minDistance = 10f)
    {
        List<Transform> safeSpawns = new List<Transform>();

        foreach (Transform spawn in spawnPoints)
        {
            if (spawn == null) continue;

            bool isSafe = true;

            foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
            {
                Health playerHealth = playerObj.GetComponent<Health>();

                // FIX — ahora sí funciona correctamente
                if (playerHealth == null || playerHealth.IsDead())
                    continue;

                float distance = Vector3.Distance(spawn.position, playerObj.transform.position);
                if (distance < minDistance)
                {
                    isSafe = false;
                    break;
                }
            }

            if (isSafe)
                safeSpawns.Add(spawn);
        }

        if (safeSpawns.Count == 0)
            return GetFarthestSpawnPoint();

        return safeSpawns[Random.Range(0, safeSpawns.Count)];
    }

    public Transform GetRandomSpawnPoint()
    {
        List<Transform> validSpawns = new List<Transform>();
        foreach (Transform spawn in spawnPoints)
            if (spawn != null) validSpawns.Add(spawn);

        if (validSpawns.Count == 0)
        {
            Debug.LogError("No hay spawn points válidos.");
            return null;
        }

        return validSpawns[Random.Range(0, validSpawns.Count)];
    }

    private Transform GetFarthestSpawnPoint()
    {
        Transform farthestSpawn = spawnPoints[0];
        float maxDistance = 0f;

        foreach (Transform spawn in spawnPoints)
        {
            if (spawn == null) continue;

            float totalDistance = 0f;
            int playerCount = 0;

            foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
            {
                Health playerHealth = playerObj.GetComponent<Health>();
                if (playerHealth != null && !playerHealth.IsDead())
                {
                    totalDistance += Vector3.Distance(spawn.position, playerObj.transform.position);
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
            if (spawn == null) continue;

            bool isSafe = true;

            if (Application.isPlaying)
            {
                foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
                {
                    Health playerHealth = playerObj.GetComponent<Health>();
                    if (playerHealth != null && !playerHealth.IsDead())
                    {
                        float distance = Vector3.Distance(spawn.position, playerObj.transform.position);
                        if (distance < minDistanceBetweenPlayers)
                        {
                            isSafe = false;
                            break;
                        }
                    }
                }
            }

            Gizmos.color = isSafe ? safeColor : unsafeColor;
            Gizmos.DrawSphere(spawn.position, gizmoRadius);

            Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.2f);
            Gizmos.DrawWireSphere(spawn.position, minDistanceBetweenPlayers);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
