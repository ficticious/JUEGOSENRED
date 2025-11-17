using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSound : MonoBehaviour
{
    public AudioClip clip;
    public float minDist, maxDist;

    private void Start()
    {
        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Logarithmic;
        src.minDistance = minDist;
        src.maxDistance = maxDist;

        src.PlayOneShot(clip);
        Destroy(gameObject, clip.length);
    }
}
