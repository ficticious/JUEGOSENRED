using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeWeapon : MonoBehaviourPun, IPunObservable
{
    [Header("UI")]
    [SerializeField] private Image crosshairUI;

    [Space]
    [Header("Weapon List")]
    [SerializeField] private List<GameObject> weapons = new List<GameObject>();
    [Min(1)] public int killsPerChange;

    public int currentWeaponIndex = 0;
    private Weapon currentWeapon;

    private int lastKillCheckpoint = 0;

    private void Start()
    {
        if (weapons.Count > 0) EquipWeapon(0);

        if (photonView.IsMine)
        {
            photonView.RPC("RPC_EquipWeapon", RpcTarget.OthersBuffered, currentWeaponIndex);
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (photonView.IsMine)
        {
            CheckKillsForWeaponChange();

            if (Input.GetKeyUp(KeyCode.Q)) NextWeapon();
        }
    }

    public void EquipWeapon(int index)
    {
        for (int i = 0; i < weapons.Count; i++)
            weapons[i].SetActive(i == index);

        currentWeaponIndex = index;
        currentWeapon = weapons[index].GetComponent<Weapon>();

        //Debug.Log("Equipped: " + currentWeapon.name);

        if (currentWeapon != null && crosshairUI != null)
            crosshairUI.sprite = currentWeapon.crosshair;
    }

    private void NextWeapon()
    {
        int nextIndex = currentWeaponIndex + 1;

        if (nextIndex >= weapons.Count) nextIndex = 0;

        EquipWeapon(nextIndex);

        //photonView.RPC("RPC_EquipWeapon", RpcTarget.AllBuffered, currentWeaponIndex);
    }

    //[PunRPC]
    //private void RPC_EquipWeapon(int index)
    //{
    //    currentWeaponIndex = index;
    //    EquipWeapon(currentWeaponIndex);

    //    Debug.Log("Jugador remoto equipó: " + currentWeapon.name);
    //}

    private void CheckKillsForWeaponChange()
    {
        int currentKills = GameManager.instance.kills;
        //int lastKillCheckpoint = 0;

        if (currentKills >= lastKillCheckpoint + killsPerChange)
        {
            lastKillCheckpoint = currentKills;
            NextWeapon();
        }
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentWeaponIndex);
        }
        else
        {
            int receivedIndex = (int)stream.ReceiveNext();

            if (receivedIndex != currentWeaponIndex)
                EquipWeapon(receivedIndex);
        }
    }
}
