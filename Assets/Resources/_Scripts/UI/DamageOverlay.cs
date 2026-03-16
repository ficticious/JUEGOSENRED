using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageOverlay : MonoBehaviour
{
    [Header("Overlay")]
    public Image overlayImage;

    [Header("Settings")]
    public float maxAlpha = 0.6f;
    public float fadeInTime = 0.05f;
    public float fadeOutTime = 0.5f;

    private Coroutine currentRoutine;

    public void ShowDamage()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(DamageRoutine());
    }

    public void DeactivateOverlay()
    {
        SetAlpha(0);
    }

    private IEnumerator DamageRoutine()
    {
        yield return FadeAlpha(0f, maxAlpha, fadeInTime);

        yield return FadeAlpha(maxAlpha, 0f, fadeOutTime);
    }

    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / duration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(to);
    }

    private void SetAlpha(float a)
    {
        if (!overlayImage) return;

        Color c = overlayImage.color;
        c.a = a;
        overlayImage.color = c;
    }
}
