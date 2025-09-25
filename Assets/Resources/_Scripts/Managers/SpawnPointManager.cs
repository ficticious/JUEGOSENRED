using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;


public class SpawnPointManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static SpawnPointManager Instance;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float minDistanceBetweenPlayers = 10f;

    [Header("Debug Settings")]
    public bool showDebugGizmos = true;
    public float debugMinDistance = 10f;
    public Color safeColor = Color.green;
    public Color unsafeColor = Color.red;
    public float gizmoRadius = 1.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

    public Transform GetSafeSpawnPoint(float minDistance = 10f)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No hay puntos de spawn disponibles!");
            return null;
        }

        List<Transform> safeSpawns = new List<Transform>();

        foreach (Transform spawn in spawnPoints)
        {
            if (spawn == null) continue;

            bool isSafe = true;

            foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
            {
                Health playerHealth = playerObj.GetComponent<Health>();

                if (playerHealth != null && !playerHealth.IsDead() && playerHealth.GetCurrentHealth() > 0)
                {
                    float distance = Vector3.Distance(spawn.position, playerObj.transform.position);
                    if (distance < minDistance)
                    {
                        isSafe = false;
                        break;
                    }
                }
            }

            if (isSafe) safeSpawns.Add(spawn);
        }

        if (safeSpawns.Count == 0) return GetFarthestSpawnPoint();

        return safeSpawns[Random.Range(0, safeSpawns.Count)];
    }

    
    public Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No hay puntos de spawn disponibles para spawn aleatorio!");
            return null;
        }

        
        List<Transform> validSpawns = new List<Transform>();
        foreach (Transform spawn in spawnPoints)
        {
            if (spawn != null) validSpawns.Add(spawn);
        }

        if (validSpawns.Count == 0)
        {
            Debug.LogError("No hay spawn points válidos!");
            return null;
        }
       
        return validSpawns[Random.Range(0, validSpawns.Count)];
    }

    private Transform GetFarthestSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return null;

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

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        GameObject localPlayer = FindLocalPlayer();

        if (localPlayer != null)
        {
            SpawnLocalPlayer(localPlayer);
        }
    }

    private GameObject FindLocalPlayer()
    {
        foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView pv = playerObj.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                return playerObj;
            }
        }
        return null;
    }

    private void SpawnLocalPlayer(GameObject player)
    {
        Transform spawnPoint = GetSafeSpawnPoint(minDistanceBetweenPlayers);

        if (spawnPoint != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.photonView.RPC("SetRespawnPosition", RpcTarget.All,
                    spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z,
                    spawnPoint.rotation.x, spawnPoint.rotation.y, spawnPoint.rotation.z, spawnPoint.rotation.w);
            }

            Debug.Log($"Jugador spawneado en: {spawnPoint.name}");
        }
        else
        {
            Debug.LogWarning("No se pudo encontrar un punto de spawn seguro");
        }
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
