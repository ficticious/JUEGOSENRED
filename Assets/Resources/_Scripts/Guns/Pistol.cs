using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Pistol : Weapon
{
    private void Start()
    {
        originalPosition = transform.parent.localPosition;

        recoilLength = 0.1f;
        recoverLength = 1 / fireRate * recoverPercent;
    }

    //-----------------------  SEMI-AUTOMATICA  ------------------------
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
        recoiling = true;
        recovering = false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, hitMask, QueryTriggerInteraction.Ignore))
        {
            if (hitVFX) Photon.Pun.PhotonNetwork.Instantiate(Path.Combine("_Prefabs", "VFX", hitVFX.name), hit.point, Quaternion.identity);
            DoDamage(hit, damage);
        }
    }
}
