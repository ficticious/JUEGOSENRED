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

        //if (!PhotonNetwork.InLobby)
        //    PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        tryingToReconnect = false;

        retryDelay = Mathf.Min(retryDelay * 2f, maxRetryDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //errorText.SetActive(!tryingToReconnect);
    }

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
}
