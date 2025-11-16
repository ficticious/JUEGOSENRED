using Photon.Pun;
using UnityEngine;

public class PlayerAnimatorSync : MonoBehaviourPun, IPunObservable
{
    public Animator anim;

    private bool remoteMoving;
    private bool remoteRunning;
    private bool jumpTriggerReceived;

    private void Awake()
    {
        //anim = GetComponentInChildren<Animator>();
        Debug.Log(anim.gameObject.name);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(anim.GetBool("Moving"));
            stream.SendNext(anim.GetBool("Running"));
            stream.SendNext(anim.GetBool("Jumping"));

            Debug.Log(remoteMoving);
        }
        else
        {
            remoteMoving = (bool)stream.ReceiveNext();
            remoteRunning = (bool)stream.ReceiveNext();
            jumpTriggerReceived = (bool)stream.ReceiveNext();

            anim.SetBool("Moving", remoteMoving);
            anim.SetBool("Running", remoteRunning);

            if (jumpTriggerReceived)
                anim.SetTrigger("Jumping");

            Debug.Log(jumpTriggerReceived);

        }
    }
}
