using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWeapon : MonoBehaviourPun
{
    [Header("UI")]
    [SerializeField] private Image crosshairUI;

    [Space]
    [SerializeField] private List<GameObject> weapons = new List<GameObject>();

    private int currentWeaponIndex = 0;
    private Weapon currentWeapon;

    private int killCount = 0;
    private int killsPerChange = 3;

    private void Start()
    {
        if (weapons.Count > 0)
            EquipWeapon(0);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            AddKill();
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

    // Llamar a esto cuando el jugador haga una kill
    public void AddKill()
    {
        killCount++;

        if (killCount % killsPerChange == 0)
        {
            NextWeapon();
        }
    }
}

/*
[PunRPC]
void EquipWeaponRPC(int weaponIndex)
{
    for (int i = 0; i < weapons.Count; i++)
        weapons[i].SetActive(i == weaponIndex);

    currentWeaponIndex = weaponIndex;
    currentWeapon = weapons[weaponIndex].GetComponent<Weapon>();

    if (photonView.IsMine && crosshairUI != null)
        crosshairUI.sprite = currentWeapon.crosshair;
}

// Si quisieras sincronizar el cambio de arma vía RPC
private void EquipWeapon(int index)
{
    photonView.RPC("EquipWeaponRPC", RpcTarget.AllBuffered, index);
}
*/
