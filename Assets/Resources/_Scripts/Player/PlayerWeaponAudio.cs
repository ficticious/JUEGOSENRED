using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponAudio : MonoBehaviour
{
    public ChangeWeapon changeWeapon;

    [PunRPC]
    public void PlayFireSound()
    {
        if (changeWeapon == null) return;
        Weapon currentWeapon = changeWeapon.GetCurrentWeapon();
        if (currentWeapon == null) return;
        if (currentWeapon.audioSource == null) return;
        if (currentWeapon.fireSound == null || currentWeapon.fireSound.Length == 0) return;

        if (currentWeapon.fireSound.Length > 1)
            currentWeapon.audioSource.PlayOneShot(currentWeapon.fireSound[Random.Range(0, currentWeapon.fireSound.Length)]);
        else
            currentWeapon.audioSource.PlayOneShot(currentWeapon.fireSound[0]);
    }
}
