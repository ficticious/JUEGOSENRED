using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviourPunCallbacks, IPunObservable
{
    public static readonly List<Health> AllActivePlayers = new List<Health>();

    [Header("Parameters")]
    public float maxHealth = 100f;
    private float currentHealth;

    public bool IsDead { get; private set; }

    // Patrón Observer: Eventos a los que otros scripts (UI, Respawn) se van a suscribir
    public event Action<float, float> OnHealthChanged;
    public event Action<int> OnDied;
    public event Action OnRespawned;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        AllActivePlayers.Add(this); // Nos registramos globalmente
    }

    public override void OnDisable()
    {
        base.OnDisable();
        AllActivePlayers.Remove(this); // Nos damos de baja
    }

    public void TakeDamage(float damage, int attackerId = -1)
    {
        if (IsDead || !photonView.IsMine) return; // Solo el dueño dicta su propio daño
        photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damage, attackerId);
    }

    [PunRPC]
    private void RPC_TakeDamage(float damage, int attackerId)
    {
        if (IsDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        // Dispara el evento para que la UI se actualice automáticamente
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die(attackerId);
        }
    }

    private void Die(int attackerId)
    {
        IsDead = true;
        OnDied?.Invoke(attackerId); // Avisa a los managers de Respawn, Stats, etc.
    }

    public void ResetHealth()
    {
        IsDead = false;
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnRespawned?.Invoke();
    }

    public void Heal(float healAmount)
    {
        if (IsDead || !photonView.IsMine) return;
        photonView.RPC(nameof(RPC_Heal), RpcTarget.All, healAmount);
    }

    [PunRPC]
    private void RPC_Heal(float healAmount)
    {
        if (IsDead) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }


    // Opcional: Para sincronizar la vida exacta para jugadores que entran tarde (Late Joiners)
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
        }
        else
        {
            currentHealth = (float)stream.ReceiveNext();
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}