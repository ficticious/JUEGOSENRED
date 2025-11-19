using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionHandler : MonoBehaviourPunCallbacks
{
    public static ConnectionHandler instance;

    [Header("Overlay UI")]
    public TextMeshProUGUI overlayText;
    public GameObject errorText;
    public GameObject popupPanel;
    public TextMeshProUGUI popupText;

    private float timer;
    private const float overlayRefresh = 0.5f;

    private bool tryingToReconnect = false;
    private float retryDelay = 1f;
    private const float maxRetryDelay = 10f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= overlayRefresh)
        {
            timer = 0f;
            UpdateOverlay();
        }

        if (!PhotonNetwork.IsConnected && !tryingToReconnect)
        {
            StartReconnect();
        }
    }

    private void StartReconnect()
    {
        tryingToReconnect = true;
        ShowPopup($"Conexión perdida.\nReconectando en {retryDelay:0}s...");
        Invoke(nameof(TryReconnect), retryDelay);
    }

    private void TryReconnect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        tryingToReconnect = false;
        retryDelay = 1f;

        ShowPopup("Reconexión exitosa.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        tryingToReconnect = false;

        retryDelay = Mathf.Min(retryDelay * 2f, maxRetryDelay);

        ShowPopup($"Desconectado: {cause}\nReintentando...");

        StartReconnect();

        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //errorText.SetActive(!tryingToReconnect);
    }




    // ----------------- FEEDBACK DE ROOM -----------------------

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ShowPopup($"{newPlayer.NickName} se unió a la partida");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ShowPopup($"{otherPlayer.NickName} abandonó la partida");

        if (PhotonNetwork.CurrentRoom.PlayerCount < 3)
        {
            ShowPopup("La partida termina: jugadores insuficientes.");
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
            ShowPopup("Sos el nuevo Host (Master Client)");
        else
            ShowPopup($"Nuevo Host: {newMasterClient.NickName}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ShowPopup($"Error: La sala no existe");
        //ShowPopup($"Error al entrar a la sala:\n{message}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ShowPopup($"Error al crear la sala:\n{message}");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        ShowPopup($"No se encontró sala.\nCreando una...");
    }







    // -----------------------  UI  --------------------------------------------
    private void UpdateOverlay()
    {
        if (overlayText == null) return;

        string connectionState = PhotonNetwork.NetworkClientState.ToString();
        int ping = PhotonNetwork.GetPing();

        string roomName = PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "-";
        string players = PhotonNetwork.InRoom ? $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}" : "-";

        overlayText.text =
            $"<b>Network</b>\n" +
            $"Ping: {ping} ms\n" +
            $"Estado: {connectionState}\n" +
            $"Backoff: {retryDelay}s\n" +
            $"Sala: {roomName}\n" +
            $"Jugadores: {players}\n";
    }

    public void ShowPopup(string msg)
    {
        if (popupPanel == null || popupText == null) return;

        popupPanel.SetActive(true);
        popupText.text = msg;

        CancelInvoke(nameof(HidePopup));
        Invoke(nameof(HidePopup), 3f);
    }

    private void HidePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }
    //------------------------------------------------------------------------

}
