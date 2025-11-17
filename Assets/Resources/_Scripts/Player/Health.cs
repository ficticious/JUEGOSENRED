using Photon.Pun;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviourPunCallbacks
{
    [Header("Parameters")]
    public float health;
    //private float respawnTime;
    private float maxHealth = 100;

    [Header("UI")]
    public TextMeshProUGUI healthText;

    public PlayerSetup playerSetup;
    public bool isLocalPlayer;
    public bool isDead = false;
    private bool canRespawn = true;


    void Start()
    {
        health = maxHealth;
        isDead = false;
        playerSetup = GetComponent<PlayerSetup>();
        isLocalPlayer = photonView.IsMine;
        UpdateUI(healthText, health);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(30, -1);
        }

        canRespawn = !GameManager.instance.gameFinished;
    }



    // -----------------  FUNCIONES NORMALES  ----------------------------

    public void RespawnPlayer()
    {
        if (!photonView.IsMine) return;

        Transform spawnPoint = SpawnPointManager.Instance.GetSafeSpawnPoint(10f);

        if (spawnPoint == null) spawnPoint = SpawnPointManager.Instance.GetRandomSpawnPoint();

        else
        {
            photonView.RPC("SetRespawnPosition", RpcTarget.All,
                          spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z,
                          spawnPoint.rotation.x, spawnPoint.rotation.y, spawnPoint.rotation.z, spawnPoint.rotation.w);
        }

        photonView.RPC("CompleteRespawn", RpcTarget.All);
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;

        health = Mathf.Min(maxHealth, health + healAmount);
        UpdateUI(healthText, health);
    }

    public void ResetHealth()
    {
        health = maxHealth;
        UpdateUI(healthText, health);
    }

    public void UpdateUI(TextMeshProUGUI text, float value)
    {
        if (text != null)
            text.text = value.ToString("F1");
    }
    //-----------------------------------------------------------------






    // -----------------  FUNCIONES PUN RPC  ----------------------------

    [PunRPC]
    public void Die(int attackerId = -1)
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} murió.");

        if (photonView.IsMine)
        {
            playerSetup.DisablePlayer();
            playerSetup.EnableLocalCamera(false);

            SpectatorCameraManager.Instance.EnableSpectator();

            GameManager.instance.deaths++;
            GameManager.instance.SetHashes();

            //GulagManager.Instance.AddPlayerToGulag(this);

            StartCoroutine(RespawnCoroutine());
        }

        if (attackerId != -1 &&
            PhotonNetwork.LocalPlayer.ActorNumber == attackerId &&
            photonView.Owner.ActorNumber != attackerId)
        {
            GameManager.instance.kills++;
            GameManager.instance.SetHashes();
        }
    }

    [PunRPC]
    public void TakeDamage(float damage, int attackerId)
    {
        if (isDead || health <= 0) return;

        health -= damage;
        health = Mathf.Max(0, health);
        UpdateUI(healthText, health);

        if (health <= 0)
        {
            photonView.RPC("Die", RpcTarget.All, attackerId);
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        TakeDamage(damage, -1);
    }

    [PunRPC]
    public void SetRespawnPosition(float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float rotW)
    {
        transform.position = new Vector3(posX, posY, posZ);
        transform.rotation = new Quaternion(rotX, rotY, rotZ, rotW);
    }

    [PunRPC]
    public void CompleteRespawn()
    {
        if (canRespawn) SpectatorCameraManager.Instance.DisableSpectator();
        ResetHealth();
        isDead = false;

        if (photonView.IsMine)
        {
            if (playerSetup != null && canRespawn)
            {
                playerSetup.EnablePlayer();
                playerSetup.EnableLocalCamera(true);

                Debug.Log($"{gameObject.name} ha respawneado");
            }
        }

    }
    //----------------------------------------------------------------------------------






    // -----------------  FUNCIONES VARIABLES  ----------------------------

    public float GetCurrentHealth()
    {
        return health;
    }

    public bool IsDead()
    {
        return isDead;
    }
    //----------------------------------------------------






    // -----------------  COROUTINES  ----------------------------
    public IEnumerator RespawnAfterGulag()
    {
        yield return new WaitForSeconds(1.5f);

        if (!photonView.IsMine) yield break;

        SpectatorCameraManager.Instance.DisableSpectator();
        ResetHealth();
        isDead = false;

        Transform spawn = SpawnPointManager.Instance.GetRandomSpawnPoint();

        photonView.RPC("SetRespawnPosition", RpcTarget.All,
                      spawn.position.x, spawn.position.y, spawn.position.z,
                      spawn.rotation.x, spawn.rotation.y, spawn.rotation.z, spawn.rotation.w);

        photonView.RPC("CompleteRespawn", RpcTarget.All);
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(SpawnManager.instance.respawnTime);
        if (!photonView.IsMine) yield break;
        RespawnPlayer();
    }
    //------------------------------------------------------------
}