using Photon.Pun;
using System.Collections;
using UnityEngine;

public static class GulagPlayerHandler
{
    public static void StartSpectatorDelay(Health player, float delay, PhotonView pv)
    {
        player.StartCoroutine(SpectatorDelayCoroutine(player, delay, pv));
    }

    private static IEnumerator SpectatorDelayCoroutine(Health player, float delay, PhotonView pv)
    {
        Debug.Log($"[PLAYER] Jugador {player.photonView.ViewID} en spectator por {delay}s");

        if (SpectatorCameraManager.Instance != null)
        {
            SpectatorCameraManager.Instance.EnableSpectator();
        }

        yield return new WaitForSeconds(delay);

        pv.RPC("RPC_AddPlayerToGulag", RpcTarget.MasterClient, player.photonView.ViewID);
    }

    public static void TeleportToWaiting(int viewID, Transform spawnPoint)
    {
        Health player = PhotonView.Find(viewID)?.GetComponent<Health>();
        if (player == null)
        {
            Debug.LogError($"[PLAYER] Jugador {viewID} no encontrado");
            return;
        }

        TeleportPlayer(player, spawnPoint);
        ResetPlayer(player);

        if (player.photonView.IsMine)
        {
            EnablePlayer(player);
        }
    }

    public static void SetupDuelPlayer(Health player, Transform spawnPoint)
    {
        TeleportPlayer(player, spawnPoint);
        ResetPlayer(player);

        if (player.photonView.IsMine)
        {
            EnablePlayer(player);
        }
    }

    public static void WaitWithTimeout(Health player, float timeout, GulagQueue queue, PhotonView pv)
    {
        player.StartCoroutine(WaitForGulagOrRespawn(player, timeout, queue, pv));
    }

    private static IEnumerator WaitForGulagOrRespawn(Health player, float timeout, GulagQueue queue, PhotonView pv)
    {
        float timeWaited = 0f;
        int playerViewID = player.photonView.ViewID;

        Debug.Log($"[PLAYER] Jugador {playerViewID} esperando máximo {timeout}s");

        while (timeWaited < timeout)
        {
            if (queue.IsGulagFree())
            {
                Debug.Log($"[PLAYER] Gulag liberado para {playerViewID}");
                pv.RPC("RPC_AddPlayerToGulag", RpcTarget.MasterClient, playerViewID);
                yield break;
            }

            if (!queue.IsGulagActive && queue.GetCurrentWaitingPlayer() == playerViewID)
            {
                Debug.Log($"[PLAYER] Jugador {playerViewID} ya en espera");
                yield break;
            }

            yield return new WaitForSeconds(1f);
            timeWaited += 1f;
        }

        Debug.Log($"[PLAYER] Timeout para {playerViewID}, respawn directo");
        RespawnToMap(player);
    }

    public static void RespawnAfterDelay(Health player, float delay)
    {
        Debug.Log($"[PLAYER] RespawnAfterDelay iniciado para ViewID {player.photonView.ViewID}, delay: {delay}s");
        player.StartCoroutine(RespawnDelayCoroutine(player, delay));
    }

    private static IEnumerator RespawnDelayCoroutine(Health player, float delay)
    {
        Debug.Log($"[PLAYER] Esperando {delay}s para respawn de {player.photonView.ViewID}");

        yield return new WaitForSeconds(delay);

        if (!player.photonView.IsMine)
        {
            Debug.Log($"[PLAYER] No es mi jugador, saliendo");
            yield break;
        }

        Debug.Log($"[PLAYER] Ejecutando respawn para {player.photonView.ViewID}");
        RespawnToMap(player);
    }

    private static void RespawnToMap(Health player)
    {
        if (!player.photonView.IsMine) return;

        Debug.Log($"[PLAYER] Respawneando {player.photonView.ViewID} al mapa");

        if (SpectatorCameraManager.Instance != null)
        {
            SpectatorCameraManager.Instance.DisableSpectator();
        }

        player.ResetHealth();
        player.isDead = false;

        Transform spawn = SpawnPointManager.Instance.GetRandomSpawnPoint();

        player.photonView.RPC("SetRespawnPosition", RpcTarget.All,
            spawn.position.x, spawn.position.y, spawn.position.z,
            spawn.rotation.x, spawn.rotation.y, spawn.rotation.z, spawn.rotation.w);

        player.photonView.RPC("CompleteRespawn", RpcTarget.All);

        Debug.Log($"[PLAYER] Respawn completado para {player.photonView.ViewID}");
    }

    private static void ResetPlayer(Health player)
    {
        player.ResetHealth();
        player.isDead = false;
    }

    private static void TeleportPlayer(Health player, Transform spawn)
    {
        if (spawn == null)
        {
            Debug.LogError("[PLAYER] Spawn point NULL");
            return;
        }

        player.photonView.RPC("SetRespawnPosition", RpcTarget.All,
            spawn.position.x, spawn.position.y, spawn.position.z,
            spawn.rotation.x, spawn.rotation.y, spawn.rotation.z, spawn.rotation.w);
    }

    private static void EnablePlayer(Health player)
    {
        player.playerSetup.EnablePlayer();
        player.playerSetup.EnableLocalCamera(true);

        if (SpectatorCameraManager.Instance != null)
        {
            SpectatorCameraManager.Instance.DisableSpectator();
        }
    }
}
