using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWeapon : MonoBehaviour
{
    [SerializeField] GameObject pistol;
    [SerializeField] GameObject rifle;
    [SerializeField] private Image crosshairUI;

    private Weapon currentWeapon;

    private void Start()
    {
        EquipWeapon(pistol);
    }

    private void Update()
    {
        Change();
    }

    private void Change()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (currentWeapon == pistol.GetComponent<Weapon>())
                EquipWeapon(rifle);
            else
                EquipWeapon(pistol);
        }
    }

    private void EquipWeapon(GameObject weapon)
    {
        pistol.SetActive(weapon == pistol);
        rifle.SetActive(weapon == rifle);

        currentWeapon = weapon.GetComponent<Weapon>();

        // Seteo el crosshair según el arma
        if (currentWeapon != null && crosshairUI != null)
        {
            crosshairUI.sprite = currentWeapon.crosshair;
        }
    }
}
