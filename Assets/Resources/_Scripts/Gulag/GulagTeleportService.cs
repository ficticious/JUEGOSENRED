using Photon.Pun;
using UnityEngine;

public class GulagTeleportService
{
    public void TeleportPlayer(Health player, Transform spawn)
    {
        if (spawn == null)
        {
            Debug.LogError("[Teleport] Spawn NULL");
            return;
        }

        player.photonView.RPC("SetRespawnPosition", RpcTarget.All,
            spawn.position.x, spawn.position.y, spawn.position.z,
            spawn.rotation.x, spawn.rotation.y, spawn.rotation.z, spawn.rotation.w);
    }
}
