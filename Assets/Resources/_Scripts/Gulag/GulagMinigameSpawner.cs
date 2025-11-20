using Photon.Pun;
using UnityEngine;

public static class GulagMinigameSpawner
{
    public static Minigame SpawnMinigame(GameObject prefab, Health player1, Health player2)
    {
        GameObject minigameObj = PhotonNetwork.InstantiateRoomObject(
            prefab.name,
            Vector3.zero,
            Quaternion.identity
        );

        Minigame minigame = minigameObj.GetComponent<Minigame>();
        minigame.Initialize(player1, player2);

        return minigame;
    }
}