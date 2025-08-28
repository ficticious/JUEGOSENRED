using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class Scoreboard : MonoBehaviour
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefab;

    private void Start()
    {
        Debug.Log("Holaaaaaa");
        foreach (Player player in PhotonNetwork.PlayerList)
        {

            AddScoreboardItem(player);
            Debug.Log("chauuuu");
        }
        
    }

    void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(scoreboardItemPrefab, container).GetComponent<ScoreboardItem>();
        item.Initialize(player);
    } 
}
