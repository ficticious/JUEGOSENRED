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

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) && inputField.text != string.Empty)
        {
            SendMessage();
        }
    }

    public void SendMessage()
    {
        string myNickname = PhotonNetwork.LocalPlayer.NickName;
        GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, inputField.text, myNickname);
        inputField.text = string.Empty;
    }

    [PunRPC]
    public void GetMessage(string receiveMessage, string senderNickname)
    {
        GameObject M = Instantiate(message, Vector3.zero, Quaternion.identity, content.transform);
        M.GetComponent<Message>().MyMessage.text = senderNickname + ": " + receiveMessage;
    }

    public void CloseChat()
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}