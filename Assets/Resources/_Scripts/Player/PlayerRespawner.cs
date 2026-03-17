using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerRespawner : MonoBehaviourPunCallbacks
{
    private Health health;
    private PlayerSetup playerSetup;
    private CharacterController cc; // O Rigidbody, dependiendo de lo que uses

    private void Awake()
    {
        health = GetComponent<Health>();
        playerSetup = GetComponent<PlayerSetup>();
        cc = GetComponent<CharacterController>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        // Nos suscribimos al evento de muerte
        health.OnDied += HandleDeath;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        health.OnDied -= HandleDeath;
    }

    private void HandleDeath(int attackerId)
    {
        if (photonView.IsMine)
        {
            playerSetup.DisablePlayer();
            playerSetup.EnableLocalCamera(false);
            SpectatorCameraManager.Instance.EnableSpectator();

            // Aquí puedes llamar a tu GameManager para las stats
            GameManager.instance.deaths++;
            GameManager.instance.SetHashes();

            if (attackerId != -1 && PhotonNetwork.LocalPlayer.ActorNumber == attackerId && photonView.Owner.ActorNumber != attackerId)
            {
                GameManager.instance.kills++;
                GameManager.instance.SetHashes();
            }

            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(SpawnManager.instance.respawnTime);

        if (GameManager.instance.gameFinished) yield break;

        Transform spawn = SpawnPointManager.Instance.GetSafeSpawnPoint(10f);
        if (spawn == null) spawn = SpawnPointManager.Instance.GetRandomSpawnPoint();

        // PUN soporta nativamente Vector3 y Quaternion
        photonView.RPC(nameof(RPC_SetRespawn), RpcTarget.All, spawn.position, spawn.rotation);
    }

    [PunRPC]
    private void RPC_SetRespawn(Vector3 position, Quaternion rotation)
    {
        if (cc != null) cc.enabled = false;

        transform.position = position;
        transform.rotation = rotation;

        if (cc != null) cc.enabled = true;

        if (playerSetup.movementScript != null && playerSetup.movementScript.rb != null)
        {
            playerSetup.movementScript.rb.velocity = Vector3.zero;
        }

        health.ResetHealth();
        SpectatorCameraManager.Instance.DisableSpectator();

        if (photonView.IsMine)
        {
            playerSetup.EnablePlayer();
            playerSetup.EnableLocalCamera(true);
        }
    }
}
