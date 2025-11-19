using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro; 

public class Chat : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputField; 
    public GameObject message;
    public GameObject content;

    public PlayerSetup player;

    public void SendMessage()
    {
        GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, inputField.text);
        inputField.text = string.Empty;
    }

    [PunRPC]
    public void GetMessage(string RecieveMessage)
    {
        string nickname = PhotonNetwork.NickName;
        GameObject M = Instantiate(message, Vector3.zero, Quaternion.identity, content.transform);
        M.GetComponent<Message>().MyMessage.text = nickname + ": " + RecieveMessage;
    }

    public void CloseChat()
    {
        gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

