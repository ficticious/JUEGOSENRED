using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GulagArena : MonoBehaviour
{
    [Header("Arena Settings")]
    public Transform[] gulagSpawnPoints;
    public GameObject[] wallsToDisable;
    public GameObject targetObject;

    public void StartDuel(Health player1, Health player2)
    {
        Debug.Log($"[ARENA] Iniciando duelo");

        GulagPlayerHandler.SetupDuelPlayer(player1, gulagSpawnPoints[0]);
        GulagPlayerHandler.SetupDuelPlayer(player2, gulagSpawnPoints[1]);

        SetWallsActive(false);
        SetTargetActive(true);
    }

    public void EndDuel()
    {
        Debug.Log($"[ARENA] Finalizando duelo");

        SetWallsActive(true);
        SetTargetActive(false);
    }

    private void SetWallsActive(bool active)
    {
        foreach (GameObject wall in wallsToDisable)
        {
            if (wall != null)
                wall.SetActive(active);
        }
    }

    private void SetTargetActive(bool active)
    {
        if (targetObject != null)
            targetObject.SetActive(active);
    }
}