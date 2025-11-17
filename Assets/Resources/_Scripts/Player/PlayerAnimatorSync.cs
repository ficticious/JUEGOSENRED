using Photon.Pun;
using UnityEngine;

public class PlayerAnimatorSync : MonoBehaviourPun, IPunObservable
{
    public Animator anim;

    private bool remoteMoving;
    private bool remoteRunning;
    private bool remoteJumpTrigger;

    private void Awake()
    {
        //Debug.Log("Holaaaa");
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
        
        if (anim == null)
            Debug.LogError("ERROR: PlayerAnimatorSync NO encuentra Animator.");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (anim == null) return;

        if (stream.IsWriting)
        {
            if (!photonView.IsMine) return;

            stream.SendNext(anim.GetBool("Moving"));
            stream.SendNext(anim.GetBool("Running"));

            bool jumpJustActivated = anim.GetCurrentAnimatorStateInfo(0).IsTag("JumpStart");
            stream.SendNext(jumpJustActivated);

            //Debug.Log("ENVIADO Jump:" + jumpJustActivated);
        }
        else
        {
            remoteMoving = (bool)stream.ReceiveNext();
            remoteRunning = (bool)stream.ReceiveNext();
            remoteJumpTrigger = (bool)stream.ReceiveNext();

            anim.SetBool("Moving", remoteMoving);
            anim.SetBool("Running", remoteRunning);

            if (remoteJumpTrigger)
                anim.SetTrigger("Jumping");

            //Debug.Log("Datos recibidos");

        }
    }
}
