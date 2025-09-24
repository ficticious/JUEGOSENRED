using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviourPunCallbacks
{
    [Header("Parameters")]
    [SerializeField]
    public float health;
    private float maxHealth = 100;

    [Space]
    [Header("UI")]
    public TextMeshProUGUI healthText;

    private PlayerSetup playerSetup;


    public bool isLocalPlayer;



    private void Start()
    {
        health = maxHealth;
        playerSetup = GetComponent<PlayerSetup>();
        UpdateUI(healthText, health);

        
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (health < 0) return; // ya muerto

        health -= damage;
        UpdateUI(healthText, health);

        if (health <= 0)
        {
           

            photonView.RPC("Die", RpcTarget.All);
           
           
        }
    }

    [PunRPC]
    public void Die()
    {
        Debug.Log($"{gameObject.name} murió");

        playerSetup.DisablePlayer();

        if (photonView.IsMine)
        {
            if (isLocalPlayer && health <= 0)
            {
                Connect.instance.deaths++;
                Connect.instance.SetHashes();
            }

            StartCoroutine(Respawn());
        }


    }


    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2f);

        if (!photonView.IsMine) yield break;


        Transform spawn = SpawnPointManager.Instance.GetSafeSpawnPoint(5f);


        PhotonNetwork.Destroy(gameObject);


        GameObject newPlayer = PhotonNetwork.Instantiate("PlayerPrefab", spawn.position, spawn.rotation);


        Health h = newPlayer.GetComponent<Health>();
        if (h != null)
        {
            h.ResetHealth();
        }
    }

    public void Heal(float healAmount)
    {
        if (health >= maxHealth) health = maxHealth;
        else health += healAmount;

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
}