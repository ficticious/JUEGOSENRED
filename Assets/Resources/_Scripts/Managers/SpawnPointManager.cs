using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;


public class SpawnPointManager : MonoBehaviourPunCallbacks
{
    public static SpawnPointManager Instance;

    [Header("Spawn")]
    public Transform[] spawnPoints;
    public float minDistanceBetweenPlayers = 10f;

    [Header("Debug")]
    public bool showDebugGizmos = true;
    public float debugMinDistance = 10f;
    public Color safeColor = Color.green;
    public Color unsafeColor = Color.red;
    public float gizmoRadius = 1.5f;

    
    private List<Transform> usedSpawns = new List<Transform>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ValidateSpawnPoints();
    }

    private void ValidateSpawnPoints()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No hay puntos de spawn configurados!");
            return;
        }

        List<Transform> validSpawns = new List<Transform>();
        foreach (Transform spawn in spawnPoints)
        {
            if (spawn != null) validSpawns.Add(spawn);
        }
        spawnPoints = validSpawns.ToArray();
    }

    public Transform GetSafeSpawnPoint()
    {
        List<Transform> safeSpawns = new List<Transform>();

        foreach (Transform spawn in spawnPoints)
        {
            if (spawn == null || usedSpawns.Contains(spawn)) continue;

            bool isSafe = true;

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

            if (isSafe) safeSpawns.Add(spawn);
        }

        if (safeSpawns.Count == 0) return GetFarthestSpawnPoint();

        Transform chosen = safeSpawns[Random.Range(0, safeSpawns.Count)];
        usedSpawns.Add(chosen); 
        return chosen;
    }

    public Transform GetFarthestSpawnPoint()
    {
        Transform farthestSpawn = spawnPoints[0];
        float maxDistance = 0f;

        foreach (Transform spawn in spawnPoints)
        {
            if (spawn == null || usedSpawns.Contains(spawn)) continue;

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

        usedSpawns.Add(farthestSpawn); 
        return farthestSpawn;
    }

   
    public void ReleaseSpawn(Transform spawn)
    {
        if (usedSpawns.Contains(spawn))
        {
            usedSpawns.Remove(spawn);
        }
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos || spawnPoints == null) return;

        foreach (Transform spawn in spawnPoints)
        {
            if (spawn == null) continue;

            bool isSafe = !usedSpawns.Contains(spawn);

            if (Application.isPlaying)
            {
                foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
                {
                    Health playerHealth = playerObj.GetComponent<Health>();
                    if (playerHealth != null && !playerHealth.IsDead())
                    {
                        float distance = Vector3.Distance(spawn.position, playerObj.transform.position);
                        if (distance < debugMinDistance)
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
            Gizmos.DrawWireSphere(spawn.position, debugMinDistance);
        }
    }
}