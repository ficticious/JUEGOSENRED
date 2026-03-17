using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using TMPro;
using Photon.Pun.UtilityScripts;

public class LeaderBoard : MonoBehaviour
{
    [Header("Containers")]
    public GameObject playersHolder;
    public GameObject overlay;

    [Header("Options")]
    public float refreshRate = 1f;

    [Header("UI Elements")]
    public GameObject[] slots;
    public TextMeshProUGUI[] scoreTexts;
    public TextMeshProUGUI[] nameTexts;
    public TextMeshProUGUI[] kdTexts;

    private Coroutine refreshCoroutine;
    private bool isBoardActive = false;

    private void Start()
    {
        SetBoardActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetBoardActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            SetBoardActive(false);
        }
    }

    private void SetBoardActive(bool state)
    {
        if (isBoardActive == state) return;

        isBoardActive = state;
        playersHolder.SetActive(state);
        overlay.SetActive(state);

        if (state)
        {
            RefreshUI();
            refreshCoroutine = StartCoroutine(RoutineRefresh());
        }
        else if (refreshCoroutine != null)
        {
            StopCoroutine(refreshCoroutine);
            refreshCoroutine = null;
        }
    }

    private IEnumerator RoutineRefresh()
    {
        while (isBoardActive)
        {
            yield return new WaitForSeconds(refreshRate);
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        var sortedPlayers = PhotonNetwork.PlayerList.OrderByDescending(p => p.GetScore()).ToList();

        for (int i = 0; i < slots.Length; i++)
        {
            // Si el índice actual es menor que la cantidad de jugadores, mostramos los datos
            if (i < sortedPlayers.Count)
            {
                var player = sortedPlayers[i];
                slots[i].SetActive(true);

                string pName = string.IsNullOrEmpty(player.NickName) ? "unnamed" : player.NickName;
                nameTexts[i].text = pName;
                scoreTexts[i].text = player.GetScore().ToString();

                int kills = player.CustomProperties.TryGetValue("kills", out object k) ? (int)k : 0;
                int deaths = player.CustomProperties.TryGetValue("deaths", out object d) ? (int)d : 0;

                // Interpolación de strings para evitar generar basura en memoria
                kdTexts[i].text = $"{kills}/{deaths}";
            }
            else
            {
                // Ocultamos los slots sobrantes
                if (slots[i].activeSelf) slots[i].SetActive(false);
            }
        }
    }
}
