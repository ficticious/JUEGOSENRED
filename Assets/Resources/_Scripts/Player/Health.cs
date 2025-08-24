using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Health : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] 
    protected float health;
    private float maxHealth = 100;

    [Space]
    [Header("UI")]
    public TextMeshProUGUI healthText;

    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) TakeDamage(5.23f);
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        health -= damage;

        UpdateUI(healthText, health);

        if (health <= 0 )
        {
            health = 0;
            Debug.Log("Is dead");
        }
    }
    public void UpdateUI(TextMeshProUGUI text, float value)
    {
        if (text != null)
            text.text = value.ToString("F1");
    }

    public void Heal(float healAmount)
    {
        if (health >= maxHealth)
        {
            health = maxHealth;
        }
        else health += healAmount;

        UpdateUI(healthText, health);
    }
}
