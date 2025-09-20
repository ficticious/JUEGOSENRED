using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviourPunCallbacks
{
    [Header("Parameters")]
    [SerializeField]
    protected float health;
    private float maxHealth = 100;

    [Space]
    [Header("UI")]
    public TextMeshProUGUI healthText;

    private PlayerSetup playerSetup;

    private void Start()
    {
        health = maxHealth;
        playerSetup = GetComponent<PlayerSetup>();
        UpdateUI(healthText, health);
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (health <= 0) return; // ya muerto

        health -= damage;
        UpdateUI(healthText, health);

        if (health <= 0)
        {
            health = 0;
            photonView.RPC("Die", RpcTarget.All);
        }
    }

    [PunRPC]
    public void Die()
    {
        Debug.Log($"{gameObject.name} murió");

        // Desactivar control y modelo
        playerSetup.DisablePlayer();

        // Solo el dueño (local) pide respawn
        if (photonView.IsMine)
        {
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3f); // tiempo de respawn
        SpawnpointManager.Instance.RespawnPlayer(this.photonView.Owner);
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