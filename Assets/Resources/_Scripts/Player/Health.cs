using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviourPunCallbacks
{
    [Header("Parameters")]
    public float maxHealth = 100f;
    [SerializeField] public float health;
    [Header("UI")]
    public TextMeshProUGUI healthText;
    [Header("Respawn Settings")]
    public float respawnTime = 3f;
    private PlayerSetup playerSetup;
    public bool isLocalPlayer;
    private bool isDead = false;

    void Start()
    {
        health = maxHealth;
        playerSetup = GetComponent<PlayerSetup>();
        isLocalPlayer = photonView.IsMine;
        UpdateHealthUI();
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (isDead || health <= 0) return;
        health -= damage;
        health = Mathf.Max(0, health);
        UpdateHealthUI();
        if (health <= 0 && !isDead)
        {
            photonView.RPC("Die", RpcTarget.All);
        }
    }

    [PunRPC]
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log($"{gameObject.name} ha muerto");

        if (playerSetup != null)
        {
            playerSetup.DisablePlayer();
        }

        if (photonView.IsMine)
        {
            if (isLocalPlayer)
            {
                Connect.instance.deaths++;
                Connect.instance.SetHashes();
            }
            StartCoroutine(RespawnCoroutine());
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);
        if (!photonView.IsMine) yield break;
        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        if (!photonView.IsMine) return;

        Transform spawnPoint = SpawnPointManager.Instance.GetSafeSpawnPoint(10f);
        if (spawnPoint != null)
        {
            // CAMBIO CLAVE: Usar RPC para sincronizar posición
            photonView.RPC("SetRespawnPosition", RpcTarget.All,
                          spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z,
                          spawnPoint.rotation.x, spawnPoint.rotation.y, spawnPoint.rotation.z, spawnPoint.rotation.w);
        }

        // CAMBIO CLAVE: Usar RPC para sincronizar respawn
        photonView.RPC("CompleteRespawn", RpcTarget.All);
    }

    // NUEVO RPC: Sincronizar posición de respawn
    [PunRPC]
    public void SetRespawnPosition(float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float rotW)
    {
        transform.position = new Vector3(posX, posY, posZ);
        transform.rotation = new Quaternion(rotX, rotY, rotZ, rotW);
    }

    // NUEVO RPC: Completar respawn para todos
    [PunRPC]
    public void CompleteRespawn()
    {
        ResetHealth();

        if (playerSetup != null)
        {
            playerSetup.EnablePlayer();
        }

        isDead = false;
        Debug.Log($"{gameObject.name} ha respawneado");
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;
        health = Mathf.Min(health + healAmount, maxHealth);
        UpdateHealthUI();
    }

    public void ResetHealth()
    {
        health = maxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = health.ToString("F0");
        }
    }

    public float GetCurrentHealth()
    {
        return health;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void UpdateUI(TextMeshProUGUI text, float value)
    {
        if (text != null)
            text.text = value.ToString("F1");
    }
}