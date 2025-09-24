using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    //public TMP_Text textPuntos;

    public GameObject winPanel;

    //public GameObject lobbyPanel;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
