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
    [Min(1)] public int killsPerChange;

    private int currentWeaponIndex = 0;
    private Weapon currentWeapon;


    private void Start()
    {
        if (weapons.Count > 0) EquipWeapon(0);
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            CheckKillsForWeaponChange();

            if (Input.GetKeyUp(KeyCode.Q)) NextWeapon();
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

        if (nextIndex >= weapons.Count) nextIndex = 0;

        EquipWeapon(nextIndex);
    }

    private void CheckKillsForWeaponChange()
    {
        int currentKills = GameManager.instance.kills;
        int lastKillCheckpoint = 0;

        if (currentKills >= lastKillCheckpoint + killsPerChange)
        {
            lastKillCheckpoint = currentKills;
            NextWeapon();
        }
    }
}
