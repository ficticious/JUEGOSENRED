using UnityEngine;

public class SpectatorCameraManager : MonoBehaviour
{
    public static SpectatorCameraManager Instance;

    [Header("Cameras")]
    public GameObject mainSpectatorCamera;

    public bool IsInGulagZone = false;

    private void Awake()
    {
        Instance ??= this;
        if (mainSpectatorCamera != null)
            mainSpectatorCamera.SetActive(false);
    }

    public void EnableSpectator()
    {
        if (mainSpectatorCamera != null)
            mainSpectatorCamera.SetActive(true);
    }

    public void DisableSpectator()
    {
        if (mainSpectatorCamera != null)
            mainSpectatorCamera.SetActive(false);
    }
}
