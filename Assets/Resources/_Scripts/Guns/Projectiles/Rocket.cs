using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Rocket : MonoBehaviourPunCallbacks
{
    [Header("Explosion")]
    public float explosionRadius;
    public float explosionDamage;
    public float explosionForce;
    public LayerMask damageMask;
    public GameObject explosionVFX;

    [Header("Flight")]
    public float lifetime;
    public bool useGravity = false;

    private Rigidbody rb;
    private int ownerActorNumber = -1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = useGravity;
    }

    public void Initialize(Vector3 initialVelocity, int ownerActor)
    {
        ownerActorNumber = ownerActor;
        if (rb != null)
        {
            rb.velocity = initialVelocity;
        }
        StartCoroutine(LifeTimer());
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(lifetime);
        Explode(transform.position, Vector3.up);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine) return;

        ContactPoint contact = collision.contacts[0];
        Vector3 hitPoint = contact.point;
        Vector3 hitNormal = contact.normal;

        Explode(hitPoint, hitNormal);
    }

    private void Explode(Vector3 pos, Vector3 normal)
    {
        //pos = transform.position;

        if (explosionVFX != null)
        {
            Quaternion rot = Quaternion.LookRotation(normal*0.5f);
            PhotonNetwork.Instantiate(Path.Combine("_Prefabs", "VFX", explosionVFX.name), pos, rot);
        }

        Collider[] hits = Physics.OverlapSphere(pos, explosionRadius, damageMask, QueryTriggerInteraction.Ignore);

        foreach (Collider c in hits)
        {
            Rigidbody otherRb = c.attachedRigidbody;
            if (otherRb != null)
            {
                otherRb.AddExplosionForce(explosionForce, pos, explosionRadius, 1.0f, ForceMode.Impulse);
            }

            PhotonView pv = c.GetComponent<PhotonView>();
            if (pv != null && pv != photonView)
            {
                // if (pv.Owner != null && pv.Owner.ActorNumber == ownerActorNumber) continue;

                Vector3 explosionCenter = transform.position;
                float distance = Vector3.Distance(explosionCenter, c.transform.position);
                float t = Mathf.Clamp01(distance / explosionRadius);
                float dmgToApply = Mathf.Lerp(explosionDamage, 1f, t); 

                if (dmgToApply > 0f)
                {
                    if (pv.Owner != null) pv.RPC("TakeDamage", pv.Owner, dmgToApply, PhotonNetwork.LocalPlayer.ActorNumber);

                    else pv.RPC("TakeDamage", RpcTarget.All, dmgToApply, PhotonNetwork.LocalPlayer.ActorNumber);
                }
            }
        }

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
