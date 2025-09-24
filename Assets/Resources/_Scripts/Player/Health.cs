using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviourPunCallbacks
{
    [Header("Parameters")]
    [SerializeField]
    public float health;
    private float maxHealth = 100f;

    [Space]
    [Header("UI")]
    public TextMeshProUGUI healthText;

    private PlayerSetup playerSetup;

    public bool isLocalPlayer;

    // bandera para evitar procesar la muerte varias veces
    private bool isDead = false;

    private void Start()
    {
        health = maxHealth;
        isDead = false;
        playerSetup = GetComponent<PlayerSetup>();
        UpdateUI(healthText, health);
    }

    // ahora recibe también el attackerId (actorNumber)
    [PunRPC]
    public void TakeDamage(float damage, int attackerId)
    {
        // Si ya estamos muertos, o la bandera dice que ya procesamos, no hacemos nada.
        if (isDead) return;

        // Restar vida (esto lo hace SÓLO el cliente que es dueño del PhotonView)
        health -= damage;
        UpdateUI(healthText, health);

        // Si llegamos a 0 o menos, marcamos muerte y avisamos a todos con el attackerId
        if (health <= 0f)
        {
            isDead = true;
            // Llamamos a Die una sola vez desde el dueño del avatar, y notificamos a todos
            photonView.RPC("Die", RpcTarget.All, attackerId);
        }
    }

    [PunRPC]
    public void Die(int attackerId)
    {
        // Protegemos contra ejecuciones múltiples (si por alguna razón se recibe más de un RPC)
        if (isDead == false) isDead = true;

        Debug.Log($"{gameObject.name} murió");

        playerSetup.DisablePlayer();

        if (photonView.IsMine)
        {
            if (isLocalPlayer && health <= 0f)
            {
                health = 0f;
                Connect.instance.deaths++;
                Connect.instance.SetHashes();
            }

            StartCoroutine(Respawn());
        }

        // SOLO el cliente del atacante suma la kill localmente (evita incrementos múltiples)
        if (PhotonNetwork.LocalPlayer.ActorNumber == attackerId)
        {
            Connect.instance.kills++;
            Connect.instance.SetHashes();
        }

        // no dejamos la vida a 100 acá; la Respawn() y ResetHealth se encargarán.
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.1f);

        Transform spawn = SpawnPointManager.Instance.GetRandomSpawnPoint();

        transform.position = spawn.position;
        transform.rotation = spawn.rotation;

        // resetear vida y bandera
        ResetHealth();
        isDead = false;

        // reactivar controles y modelo
        playerSetup.EnablePlayer();
    }

    public void Heal(float healAmount)
    {
        health = Mathf.Min(maxHealth, health + healAmount);
        UpdateUI(healthText, health);
    }

    public void ResetHealth()
    {
        health = maxHealth;
        UpdateUI(healthText, health);
    }

    public void UpdateUI(TextMeshProUGUI text, float value)
    {
        if (text != null)
            text.text = value.ToString("F1");
    }
}
