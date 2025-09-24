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


    public bool isLocalPlayer = false;



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
            if (isLocalPlayer) 
            {
                Connect.instance.deaths++;
                Connect.instance.SetHashes();
                Debug.Log(Connect.instance.deaths);
            }

            photonView.RPC("Die", RpcTarget.AllBuffered);
            health = maxHealth;
           
        }
    }

    [PunRPC]
    public void Die()
    {
        Debug.Log($"{gameObject.name} murió");

       

        playerSetup.DisablePlayer();

    

        if (photonView.IsMine)
        {
           
            StartCoroutine(Respawn());


        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.1f);

        Transform spawn = SpawnPointManager.Instance.GetRandomSpawnPoint();

        // mover jugador al spawn
        transform.position = spawn.position;
        transform.rotation = spawn.rotation;

       

        // resetear vida
        ResetHealth();


        // reactivar controles y modelo
        playerSetup.EnablePlayer();
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