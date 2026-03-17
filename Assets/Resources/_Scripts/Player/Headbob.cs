using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbob : MonoBehaviour
{
    [Header("References")]
    public Rigidbody playerController;
    public Movement movement;

    [Header("Bob Settings")]
    public float bobSpeed = 14f;
    public float bobAmount = 0.05f;

    private float defaultYPos;
    private float timer = 0f;

    void Start()
    {
        defaultYPos = transform.localPosition.y;
    }

    void Update()
    {
        HandleHeadbob();
    }

    void HandleHeadbob()
    {
        if (Mathf.Abs(playerController.velocity.x) > 0.1f || Mathf.Abs(playerController.velocity.z) > 0.1f)
        {
            if (movement.grounded)
            {
                timer += Time.deltaTime * bobSpeed;

                float newY = defaultYPos + Mathf.Sin(timer) * bobAmount;

                transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
            }
        }
        else
        {
            timer = 0f;

            float newY = Mathf.Lerp(transform.localPosition.y, defaultYPos, Time.deltaTime * 5f);
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
    }
}
