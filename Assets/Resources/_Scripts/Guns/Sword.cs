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
    //public string triggerString;

    [Header("Melee Attack Settings")]
    public float attackRadius = 1.5f;
    //public float attackRange = 2f;
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
        }

        if (recoiling) Recoil();
        if (recovering) Recover();

    }

    public override void Fire()
    {
        if (!photonView.IsMine) return;

        if (anim != null) anim.SetTrigger("Attack");

        recoiling = true;
        recovering = false;

        //Vector3 attackOrigin = camera.transform.position + camera.transform.forward * maxDistance;

        Vector3 attackOrigin = camera.transform.position;
        Vector3 attackDirection = camera.transform.forward;


        //Collider[] hits = Physics.OverlapSphere(attackOrigin, attackRadius, damageMask, QueryTriggerInteraction.Ignore);

        Collider[] hits = Physics.OverlapSphere(attackOrigin + attackDirection * maxDistance, attackRadius, damageMask, QueryTriggerInteraction.Ignore);

        foreach (Collider hit in hits)
        {
            if (hit.transform.root == transform.root) continue;

            //Vector3 hitPoint = hit.ClosestPoint(camera.transform.position + camera.transform.forward * maxDistance);
            //if (hitVFX) PhotonNetwork.Instantiate(Path.Combine("_Prefabs", "VFX", hitVFX.name), hitPoint, Quaternion.identity);

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

        //Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore))
        //{
        //    if (hitVFX) Photon.Pun.PhotonNetwork.Instantiate(Path.Combine("_Prefabs", "VFX", hitVFX.name), hit.point, Quaternion.identity);
        //    DoDamage(hit, damage);
        //}
    }


    private void OnDrawGizmosSelected()
    {
        if (camera == null) return;

        Gizmos.color = Color.red;
        Vector3 attackOrigin = camera.transform.position + camera.transform.forward * maxDistance;
        Gizmos.DrawWireSphere(attackOrigin, attackRadius);
    }
}
