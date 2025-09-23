using UnityEngine;
using UnityEngine.UI;

public class PlayerButtonController : MonoBehaviour
{
    [Header("Botones")]
    public Button morePlayersButton;
    public Button lessPlayersButton;

    [Header("Referencia a CreateAndJoin")]
    public CreateAndJoin createAndJoinScript;

    private const int MIN_PLAYERS = 2;
    private const int MAX_PLAYERS = 4;

    private void Update()
    {
        if (createAndJoinScript != null)
        {
            int currentPlayers = createAndJoinScript._maxPlayers;

            // Desactivamos el botón si llegamos a los límites
            morePlayersButton.interactable = currentPlayers < MAX_PLAYERS;
            lessPlayersButton.interactable = currentPlayers > MIN_PLAYERS;
        }
    }
}
