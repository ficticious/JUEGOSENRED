using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Input.GetMouseButtonDown(1)) 
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

        Vector3 origin = camera.transform.position;
        Vector3 direction = camera.transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            Debug.Log("Golpeaste a: " + hit.collider.name);

            if (hitVFX)
                PhotonNetwork.Instantiate(System.IO.Path.Combine("_Prefabs", "VFX", hitVFX.name), hit.point, Quaternion.identity);

            DoDamage(hit, damage);
        }

        
        Debug.DrawRay(origin, direction * maxDistance, Color.red, 0.2f);
    }
}
