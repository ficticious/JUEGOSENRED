using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelRender : MonoBehaviour
{
    [Header("Pixel Resolution")]
    public int verticalResolution = 240;

    private Camera cam;
    private RenderTexture renderTexture;
    private RawImage screenImage;
    private Canvas pixelCanvas;

    private void Start()
    {
        cam = GetComponent<Camera>();

        CreateRenderTexture();

        GameObject canvasGO = new GameObject("PixelCanvas");
        pixelCanvas = canvasGO.AddComponent<Canvas>();
        pixelCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        pixelCanvas.sortingOrder = -100;

        GameObject rawImageGO = new GameObject("PixelScreen");
        rawImageGO.transform.SetParent(canvasGO.transform, false);

        screenImage = rawImageGO.AddComponent<RawImage>();
        screenImage.texture = renderTexture;

        RectTransform rt = screenImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }

    private void CreateRenderTexture()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }

        float aspect = (float)Screen.width / Screen.height;
        int width = Mathf.RoundToInt(verticalResolution * aspect);
        int height = verticalResolution;

        renderTexture = new RenderTexture(width, height, 16);
        renderTexture.filterMode = FilterMode.Point;

        cam.targetTexture = renderTexture;
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
    }
}
