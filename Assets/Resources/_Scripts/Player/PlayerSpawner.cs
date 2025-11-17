using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    //public static PlayerSpawner Instance;

    //private void Awake()
    //{
    //    Instance = this;
    //}

    //public override void OnJoinedRoom()
    //{
    //    base.OnJoinedRoom();

    //    SpawnLocalPlayer();
    //}

    //private void SpawnLocalPlayer()
    //{
    //    GameObject newPlayer = SpawnManager.instance.SpawnPlayer();

    //    EmptyNickname();

    //    newPlayer.GetComponent<PhotonView>().RPC(
    //        "SetNickname",
    //        RpcTarget.AllBuffered,
    //        nickname
    //    );

    //    LocalPlayer();

    //    PhotonNetwork.LocalPlayer.NickName = nickname;
    //}

    //private void EmptyNickname()
    //{
    //    //if (string.IsNullOrWhiteSpace(nickname))
    //    //    nickname = "Unnamed";

    //    string numberOfPlayer = PhotonNetwork.CountOfPlayersInRooms + 1.ToString();

    //    if (string.IsNullOrWhiteSpace(nickname) || nickname.Length == 0)
    //    {
    //        nickname = nickname + numberOfPlayer;
    //    }
    //}

    //private string nickname;

    //public void SetNicknameBeforeJoining(string newName)
    //{
    //    nickname = newName;
    //}

    //private void LocalPlayer()
    //{
    //    //newPlayer.GetComponent<PlayerSetup>().IsLocalPlayer();
    //}
}
