using Photon.Pun;
using UnityEngine;

public class PlayerInputBlocker : MonoBehaviourPun
{
    public Movement movement;
    public CameraMove cameraMove;

    public bool canAttack = true;

    //public Shooting shooting;

    public void SetBlocked(bool block)
    {
        if (!photonView.IsMine) return;

        if (movement != null)
        {
            movement.enabled = !block;
            movement.rb.velocity = Vector3.zero;
        }

        if (cameraMove != null) cameraMove.enabled = !block;

        canAttack = !block;

        //if (shooting != null) shooting.enabled = !block;
    }
}
