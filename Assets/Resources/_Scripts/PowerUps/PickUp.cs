using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class PickUp : MonoBehaviourPun
{
    [Header("General Settings")]
    public Sprite icon;
    [SerializeField] protected string pickupName = "Pickup";
    [SerializeField] protected AudioClip pickupSound;
    public float respawnTime;

    private float bounceAmplitude = 0.5f;
    private float bounceSpeed = 2f;
    private bool isActive = true;
    private Vector3 startPos;

    private SpriteRenderer sprite;
    private SphereCollider col;
    private PhotonView pv;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<SphereCollider>();
        pv = GetComponent<PhotonView>();
        startPos = transform.position;
    }

    private void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bounceSpeed) * bounceAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        //if (!active) timer += Time.deltaTime;
        //if (timer >= respawnTime) SwitchActive();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (!other.CompareTag("Player")) return;

        if (!PhotonNetwork.IsMasterClient) return;

        if (other.CompareTag("Player"))
        {
            OnPickup(other.gameObject);
            if (pickupSound != null) AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            //PhotonNetwork.Destroy(this.gameObject);

            pv.RPC("RPC_SetActive", RpcTarget.All, false);

            StartCoroutine(RespawnCoroutine());
        }
    }

    protected abstract void OnPickup(GameObject player);

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);

        pv.RPC("RPC_SetActive", RpcTarget.All, true);
    }

    [PunRPC]
    public void RPC_SetActive(bool active)
    {
        isActive = active;
        //gameObject.SetActive(active);
        sprite.enabled = active;
        col.enabled = active;
    }
}
