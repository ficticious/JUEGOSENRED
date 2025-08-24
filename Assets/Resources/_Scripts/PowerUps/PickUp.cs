using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickUp : MonoBehaviour
{
    [Header("General Settings")]
    public Sprite icon;
    [SerializeField] protected string pickupName = "Pickup";
    [SerializeField] protected AudioClip pickupSound;

    private float bounceAmplitude = 0.5f;
    private float bounceSpeed = 2f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bounceSpeed) * bounceAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (other.CompareTag("Player"))
        {
            OnPickup(other.gameObject);
            if (pickupSound != null) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    protected abstract void OnPickup(GameObject player);
}
