using Photon.Pun;
using UnityEngine;

public class PlayerInputBlocker : MonoBehaviourPun
{
    public Movement movement;
    public CameraMove cameraMove;
    //public Shooting shooting;

    public void SetBlocked(bool block)
    {
        if (!photonView.IsMine) return;

        if (movement != null) movement.enabled = !block;
        if (cameraMove != null) cameraMove.enabled = !block;
        //if (shooting != null) shooting.enabled = !block;
    }
}
