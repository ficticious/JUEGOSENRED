using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWeapon : MonoBehaviourPun
{
    [Header("UI")]
    [SerializeField] private Image crosshairUI;

    [Space]
    [Header("Weapon List")]
    [SerializeField] private List<GameObject> weapons = new List<GameObject>();

    private int currentWeaponIndex = 0;
    private Weapon currentWeapon;

    private int killsPerChange = 3;
    private int lastKillCheckpoint = 0;

    

    private void Start()
    {
        if (weapons.Count > 0)
            EquipWeapon(0);

       
      
    }

    
    private void Update()
    {
        
        if (photonView.IsMine)
        {
            CheckKillsForWeaponChange();

            
        }

       
    }

    private void EquipWeapon(int index)
    {
        for (int i = 0; i < weapons.Count; i++)
            weapons[i].SetActive(i == index);

        currentWeaponIndex = index;
        currentWeapon = weapons[index].GetComponent<Weapon>();

        Debug.Log("Equipped: " + currentWeapon.name);

        if (currentWeapon != null && crosshairUI != null)
            crosshairUI.sprite = currentWeapon.crosshair;
    }

    private void NextWeapon()
    {
        int nextIndex = currentWeaponIndex + 1;

        if (nextIndex >= weapons.Count)
            nextIndex = 0;

        EquipWeapon(nextIndex);
    }

    private void CheckKillsForWeaponChange()
    {
        int currentKills = Connect.instance.kills;
        
        if (currentKills >= lastKillCheckpoint + killsPerChange)
        {
            lastKillCheckpoint = currentKills;
            NextWeapon();
        }
    }

    
}
