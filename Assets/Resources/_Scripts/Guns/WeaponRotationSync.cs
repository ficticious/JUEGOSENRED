using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRotationSync : MonoBehaviourPun
{

    public Transform cam;

    private void Update()
    {
        transform.rotation = cam.transform.rotation;
    }

    //[Header("References")]
    //[SerializeField] private Transform camera;
    //[SerializeField] private Transform weaponHolder;

    //private float cameraPitch;
    //private float networkPitch;

    //private void Start()
    //{
    //    if (camera == null)
    //        Debug.LogError("WeaponRotationSync: NO se asignó 'camera'.");

    //    if (weaponHolder == null)
    //        Debug.LogError("WeaponRotationSync: NO se asignó 'weaponHolder'.");
    //}

    //private void Update()
    //{
    //    if (photonView.IsMine)
    //    {
    //        cameraPitch = camera.localEulerAngles.x;  
    //    }
    //    else
    //    {
    //        weaponHolder.localRotation = Quaternion.Euler(networkPitch, 0f, 0f);
    //    }
    //}

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(cameraPitch);
    //    }
    //    else
    //    {
    //        networkPitch = (float)stream.ReceiveNext();
    //    }
    //}
}
