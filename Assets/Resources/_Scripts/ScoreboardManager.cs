using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

public class ScoreboardManager : MonoBehaviour
{
    public static ScoreboardManager instance;

    [Header("UI")]
    public GameObject scoreboardPanel;
    public Transform contentParent; 
    public GameObject playerRowPrefab; 

    private Dictionary<Player, GameObject> playerRows = new Dictionary<Player, GameObject>();

    void Awake()
    {
        instance = this;
        scoreboardPanel.SetActive(false);
    }

    void Update()
    {
        
        scoreboardPanel.SetActive(Input.GetKey(KeyCode.Tab));
    }

    public void InitializeScoreboard()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            AddPlayerRow(p);
        }
    }

    public void AddPlayerRow(Player photonPlayer)
    {
        if (!playerRows.ContainsKey(photonPlayer))
        {
            GameObject row = Instantiate(playerRowPrefab, contentParent);
            row.transform.Find("Player").GetComponent<TMP_Text>().text = photonPlayer.NickName;
            row.transform.Find("Kills").GetComponent<TMP_Text>().text = "0";
            row.transform.Find("Deaths").GetComponent<TMP_Text>().text = "0";

            playerRows.Add(photonPlayer, row);
        }
    }

    public void UpdateScore(Player photonPlayer)
    {
        if (playerRows.ContainsKey(photonPlayer))
        {
            GameObject row = playerRows[photonPlayer];
            PlayerSetup stats = FindPlayerSetup(photonPlayer);

            if (stats != null)
            {
                row.transform.Find("Kills").GetComponent<TMP_Text>().text = stats.kills.ToString();
                row.transform.Find("Deaths").GetComponent<TMP_Text>().text = stats.deaths.ToString();
            }
        }
    }

    private PlayerSetup FindPlayerSetup(Player photonPlayer)
    {
        foreach (PlayerSetup ps in FindObjectsOfType<PlayerSetup>())
        {
            if (ps.photonView.Owner == photonPlayer)
                return ps;
        }
        return null;
    }
}
