using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GulagQueueManager
{
    public List<int> WaitingPlayers { get; private set; } = new List<int>();

    public void AddPlayer(int viewID)
    {
        WaitingPlayers.Add(viewID);
    }

    public bool HasOnePlayer() => WaitingPlayers.Count == 1;
    public bool HasTwoPlayers() => WaitingPlayers.Count == 2;

    public int GetFirstPlayer() => WaitingPlayers[0];
    public int GetSecondPlayer() => WaitingPlayers[1];

    public int GetOpponent(int viewID)
    {
        foreach (int id in WaitingPlayers)
            if (id != viewID) return id;

        return -1;
    }

    public void Clear() => WaitingPlayers.Clear();
}
