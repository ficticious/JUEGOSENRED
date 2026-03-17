using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Pistol : Weapon
{
    private PhotonView pv;
    private PlayerInputBlocker blocker;

    private void Start()
    {
        initialPosition = transform.localPosition;

        pv = transform.root.GetComponent<PhotonView>();
        blocker = transform.root.GetComponent<PlayerInputBlocker>();

        if (pv == null)
            Debug.LogError("Pistol: No se encontró PhotonView en el Player.");
       // else Debug.Log(pv.name);
    }


    //-----------------------  SEMI-AUTOMATICA  ------------------------
    private void Update()
    {
        if (!pv.IsMine) return;

        if (Input.GetButtonDown("Fire1") && blocker.canAttack)
        {
            Fire();
        }

        Recoil();
    }

    public override void Fire()
    {
        pv.RPC("PlayFireSound", RpcTarget.All);
        if (muzzleVFX != null) PhotonNetwork.Instantiate(Path.Combine("_Prefabs", "VFX", muzzleVFX.name), muzzlePos.position, Quaternion.identity);


        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, hitMask, QueryTriggerInteraction.Ignore))
        {
            if (hitVFX) Photon.Pun.PhotonNetwork.Instantiate(Path.Combine("_Prefabs", "VFX", hitVFX.name), hit.point, Quaternion.identity);
            DoDamage(hit, damage);
        }

        ApplyRecoil();
    }
}
