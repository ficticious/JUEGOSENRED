using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class RocketLauncher : Weapon
{
    [Header("Rocket")]
    public GameObject rocketPrefab;
    public float rocketSpeed;
    public Transform spawnOffset;

    private void Start()
    {
        originalPosition = transform.parent.localPosition;
        recoilLength = 0.12f;
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

        //if (recoiling) Recoil();
        //if (recovering) Recover();
    }

    public override void Fire()
    {
        if (!photonView.IsMine) return;

        //recoiling = true;
        //recovering = false;

        Vector3 spawnPos = spawnOffset.position; //playerCamera.transform.position + playerCamera.transform.TransformVector(spawnOffset);  ----- VECTOR3 SPAWNOFFSET -----
        Quaternion spawnRot = Quaternion.LookRotation(playerCamera.transform.forward, Vector3.up);

        GameObject rocket = PhotonNetwork.Instantiate(Path.Combine("_Prefabs", "Guns & Shields", "Projectiles", rocketPrefab.name), spawnPos, spawnRot);

        Rocket rocketScript = rocket.GetComponent<Rocket>();
        if (rocketScript != null)
        {
            rocketScript.Initialize(playerCamera.transform.forward * rocketSpeed, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }
}
