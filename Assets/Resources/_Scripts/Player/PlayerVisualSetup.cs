using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerVisualSetup : MonoBehaviourPun
{
    [Header("3rd Person Model")]
    [SerializeField] private SkinnedMeshRenderer[] renderersToHide;

    private void Start()
    {
        if (photonView.IsMine)
        {
            HideLocalPlayerMesh();
        }
    }

    private void HideLocalPlayerMesh()
    {
        if (renderersToHide == null || renderersToHide.Length == 0)
        {
            renderersToHide = GetComponentsInChildren<SkinnedMeshRenderer>();
        }

        foreach (var r in renderersToHide)
        {
            r.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
    }
}
