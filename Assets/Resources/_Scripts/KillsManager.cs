//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using Photon.Pun;
//using Photon.Realtime;
//using TMPro;
//using UnityEngine.SceneManagement;
//using Hashtable = ExitGames.Client.Photon.Hashtable;

//public class KillsManager : MonoBehaviourPunCallbacks
//{
//    public static KillsManager instance;


//    [HideInInspector]
//    public int kills = 0;
//    [HideInInspector]
//    public int deaths = 0;

//    private void Awake()
//    {
//        instance = this;
//    }
//    //public void SetHashes()
//    //{
//    //    try
//    //    {
//    //        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
//    //        hash["kills"] = kills;
//    //        hash["deaths"] = deaths;

//    //        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
//    //    }
//    //    catch
//    //    {
//    //        //do nothing
//    //    }
//    //}
//}
