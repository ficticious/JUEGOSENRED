using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;

public abstract class Weapon : MonoBehaviourPunCallbacks
{
    public Camera camera;

    [Header("Weapon Config")]
    public float damage;
    public float fireRate;
    public float maxDistance;
    private float minDistance = 5f;
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



    protected void DoDamage(RaycastHit hit, float dmg)
    {
        PhotonView targetPV = hit.transform.GetComponent<PhotonView>();
        if (targetPV == null) return;

        // cálculo de daño por distancia
        float distance = Vector3.Distance(camera.transform.position, hit.point);
        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
        float damageMultiplier = Mathf.Lerp(1f, minDamagePercent, t);
        float finalDamage = dmg * damageMultiplier;

        // Enviamos el daño SÓLO al propietario del objeto impactado (evita que todos apliquen daño)
        if (targetPV.Owner != null)
        {
            // le pasamos el actorNumber del atacante para saber quién mató
            targetPV.RPC("TakeDamage", targetPV.Owner, finalDamage, PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else
        {
            // fallback: si no tiene owner (objeto de escena), aplicar a todos (o manejar distinto)
            targetPV.RPC("TakeDamage", RpcTarget.All, finalDamage, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        Debug.Log($"Hit → {finalDamage:F1} dmg (Base {dmg:F1}, Dist {distance:F1})");
    }
    public abstract void Fire();

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
