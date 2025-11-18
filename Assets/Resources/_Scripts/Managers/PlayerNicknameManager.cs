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

        //string nick = nicknameInput.text;

        //if (string.IsNullOrEmpty(nick))
        //    return;

        //PlayerPrefs.SetString(NICK_KEY, nick);
        //PlayerPrefs.Save();

        //PhotonNetwork.NickName = nick;

        //nicknamePanel.SetActive(false);
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
            Debug.LogWarning("Nickname vacío.");
            return;
        }

        if (IsNicknameTaken(newNick))
        {
            Debug.LogWarning("Ese nombre ya está en uso por otro jugador.");
            return;
        }

        PlayerPrefs.SetString(NICK_KEY, newNick);
        PlayerPrefs.Save();

        PhotonNetwork.NickName = newNick;

        UpdateLocalPlayerNameTag(newNick);

        nicknamePanel.SetActive(false);
        Debug.Log("Nuevo nickname guardado: " + newNick);
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
