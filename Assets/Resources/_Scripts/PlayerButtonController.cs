using UnityEngine;
using UnityEngine.UI;

public class PlayerButtonController : MonoBehaviour
{
    public CreateAndJoin createAndJoinScript;

    [Header("Botones")]
    public Button morePlayersButton;
    public Button lessPlayersButton;

    private const int MIN_PLAYERS = 2;
    private const int MAX_PLAYERS = 4;

    private void Update()
    {
        if (createAndJoinScript != null)
        {
            int currentPlayers = createAndJoinScript._maxPlayers;

            morePlayersButton.interactable = currentPlayers < MAX_PLAYERS;
            lessPlayersButton.interactable = currentPlayers > MIN_PLAYERS;
        }
    }
}
