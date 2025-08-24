using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    [Header("Shotgun Config")]
    public int pellets = 8;
    public float spreadAngle = 5f;

    private void Start()
    {
        originalPosition = transform.parent.localPosition;

        recoilLength = 0.1f;
        recoverLength = 1 / fireRate * recoverPercent;
    }

    //-----------------------  SEMI-AUTOMATICA  ------------------------
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
        recoiling = true;
        recovering = false;

        for (int i = 0; i < pellets; i++)
        {
            Vector3 direction = camera.transform.forward;
            direction = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            ) * direction;

            Ray ray = new Ray(camera.transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                if (hitVFX) Photon.Pun.PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);
                DoDamage(hit, damage / pellets); // daño repartido entre balas
            }
        }
    }
}
