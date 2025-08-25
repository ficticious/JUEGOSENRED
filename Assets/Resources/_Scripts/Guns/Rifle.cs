using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    [Header("OverHeat Config")]
    [SerializeField] private float overHeatTime = 1;
    [Min(0.05f)]
    [SerializeField] private float coolTime;
    private float heatTime;
    private bool canShoot = true;


    private void Start()
    {
        originalPosition = transform.parent.localPosition;

        recoilLength = 0.1f;
        recoverLength = 1 / fireRate * recoverPercent;
    }

    //-----------------------  AUTOMATICA  ------------------------
    private void Update()
    {
        if (nextFire > 0) nextFire -= Time.deltaTime;

        if (Input.GetButton("Fire1") && nextFire <= 0 && heatTime <= overHeatTime)
        {
            if (canShoot)
            {
                heatTime += 1 * Time.deltaTime;
                nextFire = 1 / fireRate;
                Fire();
            }
        }
        else if (!Input.GetButton("Fire1")) heatTime -= 0.5f * Time.deltaTime;

        if (heatTime <= 0)
        {
            heatTime = 0;
            canShoot = true;
        }

        if (heatTime > overHeatTime)
        {
            canShoot = false;
            heatTime -= 0.5f * Time.deltaTime;
        }

        if (recoiling) Recoil();
        if (recovering) Recover();
    }


    public override void Fire()
    {
        recoiling = true;
        recovering = false;

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hitVFX) Photon.Pun.PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);
            DoDamage(hit, damage);
        }
    }
}
