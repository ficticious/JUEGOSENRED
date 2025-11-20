using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class GulagQueue : MonoBehaviour
{
    public bool IsGulagActive { get; set; } = false;

    private List<int> waitingPlayerViewIDs = new List<int>();
    private int currentWaitingPlayer = -1;

    public void AddPlayer(int playerViewID, PhotonView pv, GulagArena arena, float timeout)
    {
        Debug.Log($"[QUEUE] Jugador {playerViewID} solicitando entrada");
        Debug.Log($"[QUEUE] Estado - Activo: {IsGulagActive}, Esperando: {currentWaitingPlayer}");

        
        if (currentWaitingPlayer != -1 && !IsGulagActive)
        {
            Debug.Log($"[QUEUE] Emparejando {currentWaitingPlayer} con {playerViewID}");
            int player1 = currentWaitingPlayer;
            currentWaitingPlayer = -1;
            IsGulagActive = true;
            pv.RPC("RPC_StartDuel", RpcTarget.All, player1, playerViewID);
            return;
        }

        
        if (!IsGulagActive && currentWaitingPlayer == -1)
        {
            Debug.Log($"[QUEUE] Primer jugador {playerViewID} esperando");
            currentWaitingPlayer = playerViewID;
            pv.RPC("RPC_TeleportToWaiting", RpcTarget.All, playerViewID);
            return;
        }

        
        Debug.Log($"[QUEUE] Gulag ocupado, timeout para {playerViewID}");
        pv.RPC("RPC_WaitInSpectatorWithTimeout", RpcTarget.All, playerViewID);
    }

    public void ProcessWaitingQueue(PhotonView pv)
    {
        Debug.Log($"[QUEUE] Procesando cola. Esperando: {currentWaitingPlayer}, Cola: {waitingPlayerViewIDs.Count}");

        IsGulagActive = false;

        if (waitingPlayerViewIDs.Count > 0 && currentWaitingPlayer == -1)
        {
            int nextPlayer = waitingPlayerViewIDs[0];
            waitingPlayerViewIDs.RemoveAt(0);

            currentWaitingPlayer = nextPlayer;
            pv.RPC("RPC_TeleportToWaiting", RpcTarget.All, nextPlayer);

            Debug.Log($"[QUEUE] Jugador {nextPlayer} movido a espera");
        }
    }

    public bool IsGulagFree()
    {
        return !IsGulagActive && currentWaitingPlayer == -1;
    }

    public int GetCurrentWaitingPlayer()
    {
        return currentWaitingPlayer;
    }
}
