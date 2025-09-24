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


    public float deatCheck = 100;

    [Header("VFX -- UI")]
    public GameObject hitVFX;
    public Sprite crosshair;

   

    protected void DoDamage(RaycastHit hit, float dmg)
    {
        if (hit.transform.gameObject.GetComponent<Health>())
        {
            float distance = Vector3.Distance(camera.transform.position, hit.point);
            float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
            float damageMultiplier = Mathf.Lerp(1f, minDamagePercent, t);

            float finalDamage = damage * damageMultiplier;

            hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, finalDamage);

            deatCheck -= damage;

            //Debug.Log($"Hit {hit.transform.name} → {dmg:F1} dmg");
            Debug.Log($"Hit → {finalDamage:F1} dmg (Base {dmg:F1}, Dist {distance:F1})");
        }

        if(deatCheck <= 0)
        {
               // PhotonNetwork.LocalPlayer.AddScore(100);

                Connect.instance.kills++;
                Connect.instance.SetHashes();
                deatCheck = 100;

        }
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
