using Photon.Pun;
using UnityEngine;

public static class GulagDuelFinisher
{
    public static void FinishDuel(int shooterViewID, Minigame minigame, PhotonView pv, float loserDelay)
    {
        Debug.Log($"[DUEL] Target golpeado por {shooterViewID}");

        Health winner = PhotonView.Find(shooterViewID)?.GetComponent<Health>();
        if (winner == null) return;

        Health loser = GetLoser(shooterViewID, minigame);

        Debug.Log($"[DUEL] Ganador: {winner.gameObject.name}");

        pv.RPC("RPC_DuelFinished", RpcTarget.All,
            winner.photonView.ViewID,
            loser != null ? loser.photonView.ViewID : -1,
            loserDelay);
    }

    public static void HandleDuelFinished(int winnerViewID, int loserViewID, float loserDelay, GulagArena arena)
    {
        Debug.Log($"[DUEL] Finalizado - Ganador: {winnerViewID}");

        Health winner = PhotonView.Find(winnerViewID)?.GetComponent<Health>();
        if (winner == null) return;

        arena.EndDuel();

        if (winner.photonView.IsMine)
        {
            GulagPlayerHandler.RespawnAfterDelay(winner, 1.5f);
        }

        if (loserViewID != -1)
        {
            Health loser = PhotonView.Find(loserViewID)?.GetComponent<Health>();
            if (loser != null && loser.photonView.IsMine)
            {
                GulagPlayerHandler.RespawnAfterDelay(loser, loserDelay);
            }
        }
    }

    private static Health GetLoser(int shooterViewID, Minigame minigame)
    {
        if (minigame == null) return null;

        Health player1 = minigame.GetPlayerA();
        Health player2 = minigame.GetPlayerB();

        if (player1 != null && player1.photonView.ViewID != shooterViewID)
            return player1;

        if (player2 != null && player2.photonView.ViewID != shooterViewID)
            return player2;

        return null;
    }
}