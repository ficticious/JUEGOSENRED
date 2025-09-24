using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class SpawnPointManager : MonoBehaviourPunCallbacks
{
    public static SpawnPointManager Instance;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;

    [Header("Debug Settings")]
    public float debugMinDistance = 20f;
    public Color safeColor = Color.green;
    public Color unsafeColor = Color.red;
    public float gizmoRadius = 1.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public Transform GetSafeSpawnPoint(float minDistance = 10f)
    {
        List<Transform> safeSpawns = new List<Transform>();

        foreach (var spawn in spawnPoints)
        {
            bool isSafe = true;

            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                Health h = player.GetComponent<Health>();
                if (h != null && h.health > 0)
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

        if (safeSpawns.Count == 0)
            return spawnPoints[Random.Range(0, spawnPoints.Length)];

        return safeSpawns[Random.Range(0, safeSpawns.Count)];
    }

    // NUEVO: Se llama cuando el jugador entra al mapa
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // Aseguramos que el jugador local se mueva a un spawn seguro
        GameObject localPlayer = PhotonNetwork.LocalPlayer.TagObject as GameObject;

        if (localPlayer != null)
        {
            Transform spawn = GetSafeSpawnPoint();
            localPlayer.transform.position = spawn.position;
        }
        else
        {
            Debug.LogWarning("No se encontró al jugador local para spawnear.");
        }
    }

    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        foreach (var spawn in spawnPoints)
        {
            if (spawn == null) continue;

            bool isSafe = true;
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                Health h = player.GetComponent<Health>();
                if (h != null && h.health > 0)
                {
                    float dist = Vector3.Distance(spawn.position, player.transform.position);
                    if (dist < debugMinDistance)
                    {
                        isSafe = false;
                        break;
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