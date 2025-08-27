using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

public class ScoreboardManager : MonoBehaviourPunCallbacks
{
    
    public GameObject scoreboardPanel;
    public Transform contentParent; 
    public GameObject playerRowPrefab;

    private Dictionary<Player, GameObject> playerRows = new Dictionary<Player, GameObject>();

    void Start()
    {
        scoreboardPanel.SetActive(false);

        
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            AddPlayerRow(p);
        }
    }

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Tab))
            scoreboardPanel.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Tab))
            scoreboardPanel.SetActive(false);

        
        UpdatePlayerRows();
    }

    void AddPlayerRow(Player player)
    {
        GameObject row = Instantiate(playerRowPrefab, contentParent);
        row.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = player.NickName;
        playerRows[player] = row;
    }

    void RemovePlayerRow(Player player)
    {
        if (playerRows.ContainsKey(player))
        {
            Destroy(playerRows[player]);
            playerRows.Remove(player);
        }
    }

    void UpdatePlayerRows()
    {
        foreach (var kvp in playerRows)
        {
            Player p = kvp.Key;
            GameObject row = kvp.Value;

            int kills = p.CustomProperties.ContainsKey("Kills") ? (int)p.CustomProperties["Kills"] : 0;
            int deaths = p.CustomProperties.ContainsKey("Deaths") ? (int)p.CustomProperties["Deaths"] : 0;

            row.transform.Find("Kills").GetComponent<TextMeshProUGUI>().text = kills.ToString();
            row.transform.Find("Deaths").GetComponent<TextMeshProUGUI>().text = deaths.ToString();
        }
    }

    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddPlayerRow(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerRow(otherPlayer);
    }
}