using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Shield : Health
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameObject.activeSelf)
            UpdateUI(healthText, health);

        if (health <= 0 && gameObject.activeSelf)
            Deactivate();
    }

    public void Activate()
    {
        health = 50;
        UpdateUI(healthText, health);
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
         Debug.Log("Escudo destruido!");
         gameObject.SetActive(false);  
    }
}
