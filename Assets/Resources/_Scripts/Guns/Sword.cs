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

    private void Start()
    {
        originalPosition = transform.parent.localPosition;

        recoilLength = 0.05f;
        recoverLength = 1 / fireRate * recoverPercent;
    }

    private void Update()
    {
        if (nextFire > 0) nextFire -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && nextFire <= 0)
        {
            nextFire = 1 / fireRate;
            Fire();
            if (anim != null) anim.SetTrigger("Attack");
        }
        //if (recoiling) Recoil();
        //if (recovering) Recover();
    }

    public override void Fire()
    {
        if (!photonView.IsMine) return;

        //recoiling = true;
        //recovering = false;

        Vector3 attackOrigin = playerCamera.transform.position;
        Vector3 attackDirection = playerCamera.transform.forward;

        Collider[] hits = Physics.OverlapSphere(attackOrigin + attackDirection * maxDistance, attackRadius, damageMask, QueryTriggerInteraction.Ignore);

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
            }
        }
        Debug.Log($"Sword attack hit {hits.Length} targets");
    }


    //private void OnDrawGizmosSelected()
    //{
    //    if (playerCamera == null) return;

    //    Gizmos.color = Color.red;
    //    Vector3 attackOrigin = playerCamera.transform.position + playerCamera.transform.forward * maxDistance;
    //    Gizmos.DrawWireSphere(attackOrigin, attackRadius);
    //}
}
