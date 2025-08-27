using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWeapon : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image crosshairUI;
    [Space]
    [SerializeField] private List<GameObject> weapons = new List<GameObject>();

    private int currentWeaponIndex = 0;
    private Weapon currentWeapon;

    private void Start()
    {
        if (weapons.Count > 0)
            EquipWeapon(0);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            NextWeapon();
        }
    }

    private void EquipWeapon(int index)
    {
        for (int i = 0; i < weapons.Count; i++)
            weapons[i].SetActive(i == index);

        currentWeaponIndex = index;
        currentWeapon = weapons[index].GetComponent<Weapon>();

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
}
