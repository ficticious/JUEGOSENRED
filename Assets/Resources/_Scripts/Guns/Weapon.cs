using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using System.IO;

public abstract class Weapon : MonoBehaviourPunCallbacks
{
    public Camera playerCamera;

    public LayerMask hitMask;

    [Header("Weapon Config")]
    public float damage;
    public float fireRate;
    public float maxDistance;
    public float minDistance = 5f;
    public float damageFalloffDistance;
    private float minDamagePercent = 0.2f;
    protected float nextFire;

    [Header("Recoil")]
    [Range(0, 2)]
    public float recoverPercent;
    public float recoilUp;
    public float recoilBack;
    public bool recoiling;
    public bool recovering;

    protected float recoilLength;
    protected float recoverLength;

    protected Vector3 originalPosition;
    protected Vector3 recoilVelocity = Vector3.zero;


    [Header("VFX -- UI")]
    public GameObject hitVFX;
    public Sprite crosshair;

    public abstract void Fire();

    protected void DoDamage(RaycastHit hit, float dmg)
    {
        PhotonView targetPV = hit.transform.GetComponent<PhotonView>();
        if (targetPV == null) return;
        
        float distance = Vector3.Distance(playerCamera.transform.position, hit.point);

        if (distance > damageFalloffDistance)
        {
            Debug.Log($"Hit fuera de rango ({distance:F1}m). Solo VFX.");
            return;
        }

        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
        float damageMultiplier = Mathf.Lerp(1f, minDamagePercent, t);
        float finalDamage = dmg * damageMultiplier;

        
        if (targetPV.Owner != null)
        {
            targetPV.RPC("TakeDamage", targetPV.Owner, finalDamage, PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else
        {
            targetPV.RPC("TakeDamage", RpcTarget.All, finalDamage, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        Debug.Log($"Hit → {finalDamage:F1} dmg (Base {dmg:F1}, Dist {distance:F1})");
    }

    public void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y + recoilUp, originalPosition.z - recoilBack);

        transform.parent.localPosition = Vector3.SmoothDamp(transform.parent.localPosition, finalPosition, ref recoilVelocity, recoilLength);

        if (Vector3.Distance(transform.parent.localPosition, finalPosition) < 0.01f)
        {
            recoiling = false;
            recovering = true;
        }
    }

    public void Recover()
    {
        Vector3 finalPosition = originalPosition;

        transform.parent.localPosition = Vector3.SmoothDamp(transform.parent.localPosition, finalPosition, ref recoilVelocity, recoverLength);

        if (Vector3.Distance(transform.parent.localPosition, finalPosition) < 0.01f)
        {
            recoiling = false;
            recovering = false;
        }
    }
}
