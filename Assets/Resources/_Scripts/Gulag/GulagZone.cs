using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GulagZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Health h = other.GetComponent<Health>();
        if (h != null && h.photonView.IsMine)
        {
            if (SpectatorCameraManager.Instance != null)
                SpectatorCameraManager.Instance.IsInGulagZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Health h = other.GetComponent<Health>();
        if (h != null && h.photonView.IsMine)
        {
            if (SpectatorCameraManager.Instance != null)
                SpectatorCameraManager.Instance.IsInGulagZone = false;
        }
    }
}