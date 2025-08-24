using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [Header("Move")]
    public float baseWalkSpeed = 1f;
    public float baseSprintSpeed = 4f;
    public float maxVelocityChange = 10f;
    private float walkSpeed, sprintSpeed;
    [Space]
    [Header("Jump")]
    public float jumpForce;
    public float airControl;

    private Vector2 input;
    private Rigidbody rb;

    private bool sprinting;
    private bool jumping;
    private bool grounded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        walkSpeed = baseWalkSpeed;
        sprintSpeed = baseSprintSpeed;
    }

    private void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        sprinting = Input.GetButton("Sprint");
        jumping = Input.GetButton("Jump");
    }

    private void OnTriggerStay(Collider other)
    {
        grounded = true;
    }

    private void FixedUpdate()
    {
        if (grounded) 
        {
            if (jumping)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            }
            else if (input.magnitude > 0.5f)
            { 
                rb.AddForce(CalculateMovement(sprinting ? sprintSpeed : walkSpeed), ForceMode.VelocityChange);
            }
            else
            {
                var velocity1 = rb.velocity;
                velocity1 = new Vector3(velocity1.x * 0.2f * Time.fixedDeltaTime, velocity1.y, velocity1.z * 0.2f * Time.fixedDeltaTime);
                rb.velocity = velocity1;
            }
        }
        else
        {
            if (input.magnitude > 0.5f)
            {
                rb.AddForce(CalculateMovement(sprinting ? sprintSpeed * airControl : walkSpeed * airControl), ForceMode.VelocityChange);
            }
            else
            {
                var velocity1 = rb.velocity;
                velocity1 = new Vector3(velocity1.x * 0.2f * Time.fixedDeltaTime, velocity1.y, velocity1.z * 0.2f * Time.fixedDeltaTime);
                rb.velocity = velocity1;
            }
        }

        grounded = false;
    }

    Vector3 CalculateMovement(float _speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);

        targetVelocity *= _speed;

        Vector3 velocity = rb.velocity;

        if (input.magnitude > 0.5f)
        {
            Vector3 velocityChange = (targetVelocity - velocity);

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

            velocityChange.y = 0f;

            return velocityChange;
        }
        else
        {
            return new Vector3();
        }
    }

    public void SpeedBoost(float multiplier)
    {
        if (walkSpeed < 8 || sprintSpeed < 16)
        {
            walkSpeed *= multiplier;
            sprintSpeed *= multiplier;
        }
    }
}
