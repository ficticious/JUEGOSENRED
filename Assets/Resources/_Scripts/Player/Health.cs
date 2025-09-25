using Photon.Pun;
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
    public float respawnTime = 0f;

    private PlayerSetup playerSetup;
    public bool isLocalPlayer;
    private bool isDead = false;

    void Start()
    {
        health = maxHealth;
        isDead = false;
        playerSetup = GetComponent<PlayerSetup>();
        isLocalPlayer = photonView.IsMine;
        UpdateUI(healthText, health);
    }


    [PunRPC]
    public void TakeDamage(float damage, int attackerId)
    {
        if (isDead || health <= 0) return;

        health -= damage;
        health = Mathf.Max(0, health);
        UpdateUI(healthText, health);

        if (health <= 0 && !isDead)
        {
            isDead = true;
            photonView.RPC("Die", RpcTarget.All, attackerId);
        }
    }

    
    [PunRPC]
    public void TakeDamage(float damage)
    {
        TakeDamage(damage, -1); 
    }

    [PunRPC]
    public void Die(int attackerId = -1)
    {
        if (isDead == false) isDead = true;

        Debug.Log($"{gameObject.name} murió");

        
        if (playerSetup != null)
        {
            playerSetup.DisablePlayer();
        }

        
        if (photonView.IsMine)
        {
            if (isLocalPlayer && health <= 0f)
            {
                health = 0f;
                Connect.instance.deaths++;
                Connect.instance.SetHashes();
            }
            StartCoroutine(RespawnCoroutine());
        }

        
        if (attackerId != -1 && PhotonNetwork.LocalPlayer.ActorNumber == attackerId)
        {
            Connect.instance.kills++;
            Connect.instance.SetHashes();
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

        
        if (spawnPoint == null)
        {
            spawnPoint = SpawnPointManager.Instance.GetRandomSpawnPoint();
        }

        if (spawnPoint != null)
        {
            
            photonView.RPC("SetRespawnPosition", RpcTarget.All,
                          spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z,
                          spawnPoint.rotation.x, spawnPoint.rotation.y, spawnPoint.rotation.z, spawnPoint.rotation.w);
        }

        
        photonView.RPC("CompleteRespawn", RpcTarget.All);
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
        
        ResetHealth();
        isDead = false;

      
        if (playerSetup != null)
        {
            playerSetup.EnablePlayer();
        }

        Debug.Log($"{gameObject.name} ha respawneado");
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

    
   
    public float GetCurrentHealth()
    {
        return health;
    }

    public bool IsDead()
    {
        return isDead;
    }

    
    public void DealDamage(float damage, int attackerId = -1)
    {
        photonView.RPC("TakeDamage", RpcTarget.All, damage, attackerId);
    }

    public void DealDamage(float damage)
    {
        photonView.RPC("TakeDamage", RpcTarget.All, damage);
    }
}