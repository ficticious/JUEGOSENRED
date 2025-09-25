using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MeleeWeapon : Weapon
{
    private void Start()
    {
        originalPosition = transform.parent.localPosition;

        recoilLength = 0.05f;
        recoverLength = 1 / fireRate * recoverPercent;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1")) 
        {
            Fire();
        }

        if (recoiling) Recoil();
        if (recovering) Recover();
    }

    public override void Fire()
    {
        if (!photonView.IsMine) return; 

        recoiling = true;
        recovering = false;

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hitVFX) Photon.Pun.PhotonNetwork.Instantiate(Path.Combine("_Prefabs", "VFX", hitVFX.name), hit.point, Quaternion.identity);
            DoDamage(hit, damage);
        }


        //Debug.DrawRay(origin, direction * maxDistance, Color.red, 0.2f);
    }
}
