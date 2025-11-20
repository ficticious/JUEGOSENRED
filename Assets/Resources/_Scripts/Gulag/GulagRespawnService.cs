using System.Collections;
using UnityEngine;

public class GulagRespawnService : MonoBehaviour
{
    public IEnumerator RespawnWinner(Health h)
    {
        yield return h.StartCoroutine(h.RespawnAfterGulag());
    }

    public IEnumerator RespawnLoser(Health h, float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return h.StartCoroutine(h.RespawnAfterGulag());
    }
}
