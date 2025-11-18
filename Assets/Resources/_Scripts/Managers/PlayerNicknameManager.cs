using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerNicknameManager : MonoBehaviour
{
    public TMP_InputField nicknameInput;
    public GameObject nicknamePanel;
    public TMP_Text nicknameText;

    private const string NICK_KEY = "player_nickname";

    private void Start()
    {

        if (PlayerPrefs.HasKey(NICK_KEY))
        {
            string savedNick = PlayerPrefs.GetString(NICK_KEY);
            PhotonNetwork.NickName = savedNick;
            nicknamePanel.SetActive(false);

            nicknameText.text = savedNick;
        }
        else
        {
            nicknamePanel.SetActive(true);
        }
    }

    public void ConfirmNickname()
    {
        SetNewNickname(nicknameInput.text);
        nicknameText.text = nicknameInput.text;
    }


    public void ChangeNickname()
    {
        nicknamePanel.SetActive(true);

        string newNick = nicknameInput.text;

        //SetNewNickname(newNick);
    }

    private void SetNewNickname(string newNick)
    {
        if (string.IsNullOrEmpty(newNick))
        {
            ConnectionHandler.instance.ShowPopup($"ERROR: Nickname vacío"); 
            //Debug.LogWarning("Nickname vacío.");
            return;
        }

        if (IsNicknameTaken(newNick))
        {
            ConnectionHandler.instance.ShowPopup($"ERROR: Ese nombre ya está en uso por otro jugador");

            //Debug.LogWarning("Ese nombre ya está en uso por otro jugador.");
            return;
        }

        PlayerPrefs.SetString(NICK_KEY, newNick);
        PlayerPrefs.Save();

        PhotonNetwork.NickName = newNick;

        UpdateLocalPlayerNameTag(newNick);

        nicknamePanel.SetActive(false);

        ConnectionHandler.instance.ShowPopup($"Nuevo nickname guardado: " + newNick);

        //Debug.Log("Nuevo nickname guardado: " + newNick);
    }

    private bool IsNicknameTaken(string nick)
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.NickName == nick)
                return true;
        }
        return false;
    }

    private void UpdateLocalPlayerNameTag(string nick)
    {
        PhotonView[] views = FindObjectsOfType<PhotonView>();

        foreach (var pv in views)
        {
            if (pv.IsMine)
            {
                PlayerSetup setup = pv.GetComponent<PlayerSetup>();
                if (setup != null)
                {
                    pv.RPC("SetNickname", RpcTarget.AllBuffered, nick);
                }
                break;
            }
        }
    }
}
