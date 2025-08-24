using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
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

    private void Update()
    {
        //if (recoiling) Recoil();
        //if (recovering) Recover();

        //if (nextFire > 0) nextFire -= Time.deltaTime;

        //if (Input.GetButton("Fire1") && nextFire <= 0)
        //{
        //    nextFire = 1 / fireRate;
        //    Fire();
        //}
    }

    protected void DoDamage(RaycastHit hit, float dmg)
    {
        if (hit.transform.gameObject.GetComponent<Health>())
        {
            hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, dmg);

            Debug.Log($"Hit {hit.transform.name} → {dmg:F1} dmg");
        }
    }
    public abstract void Fire();

    //protected void Fire()
    //{
    //    recoiling = true;
    //    recovering = false;

    //    Ray ray = new Ray(camera.transform.position, camera.transform.forward);

    //    RaycastHit hit;

    //    if (Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance))
    //    {
    //        PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);

    //        if (hit.transform.gameObject.GetComponent<Health>())
    //        {
    //            float distance = Vector3.Distance(camera.transform.position, hit.point);

    //            float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
    //            float damageMultiplier = Mathf.Lerp(1f, minDamagePercent, t);

    //            float finalDamage = damage * damageMultiplier;

    //            hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, finalDamage);

    //            Debug.Log($"Hit {hit.transform.name} at {distance:F1}m → {finalDamage:F1} dmg");
    //        }
    //    }
    //}

    public void Recoil()
    {
        print("Recoil");

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
        print("Recover");

        Vector3 finalPosition = originalPosition;

        transform.parent.localPosition = Vector3.SmoothDamp(transform.parent.localPosition, finalPosition, ref recoilVelocity, recoverLength);

        if (Vector3.Distance(transform.parent.localPosition, finalPosition) < 0.01f)
        {
            recoiling = false;
            recovering = false;
        }
    }
}
