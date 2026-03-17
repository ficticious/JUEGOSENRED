using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using UnityEngine.InputSystem.HID;

public class Sword : Weapon
{
    [Header("Anim")]
    public Animator anim;
    

    [Header("Melee Attack Settings")]
    public float attackRadius = 1.5f;

    public LayerMask damageMask;
    private PhotonView pv;
    private PlayerInputBlocker blocker;

    private void Start()
    {
        pv = transform.root.GetComponent<PhotonView>();
        blocker = transform.root.GetComponent<PlayerInputBlocker>();

        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        if (!pv.IsMine) return;
        if (nextFire > 0) nextFire -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && nextFire <= 0 && blocker.canAttack)
        {
            nextFire = 1 / fireRate;
            Fire();
            if (anim != null) anim.SetTrigger("Attack");
        }
    }

    public override void Fire()
    {
        if (!photonView.IsMine) return;

        Vector3 attackOrigin = playerCamera.transform.position;
        Vector3 attackDirection = playerCamera.transform.forward;

        Collider[] hits = Physics.OverlapSphere(attackOrigin + attackDirection * maxDistance, attackRadius, damageMask, QueryTriggerInteraction.Ignore);

        if (muzzleVFX != null) PhotonNetwork.Instantiate(Path.Combine("_Prefabs", "VFX", muzzleVFX.name), muzzlePos.position, Quaternion.identity);

        foreach (Collider hit in hits)
        {
            if (hit.transform.root == transform.root) continue;

            PhotonView targetPV = hit.GetComponent<PhotonView>();

            if (targetPV != null && targetPV != photonView)
            {
                targetPV.RPC("TakeDamage", targetPV.Owner, damage, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }

        if (Physics.Raycast(attackOrigin, attackDirection, out RaycastHit hitInfo, maxDistance * 1.75f, damageMask, QueryTriggerInteraction.Ignore))
        {
            if (hitVFX)
            {
                PhotonNetwork.Instantiate(Path.Combine("_Prefabs", "VFX", hitVFX.name), hitInfo.point, Quaternion.identity);
                pv.RPC("PlayFireSound", RpcTarget.All);
            }
        }
        //Debug.Log($"Sword attack hit {hits.Length} targets");
    }
}
