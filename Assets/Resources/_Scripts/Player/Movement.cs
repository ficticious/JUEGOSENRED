using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [Header("Move")]
    public float baseWalkSpeed = 2f;
    public float baseSprintSpeed = 5f;

    private float walkSpeed, sprintSpeed;

    [Header("Jump")]
    public float jumpForce = 5f;
    public float airControl = 0.5f;

    private Vector2 input;
    private Rigidbody rb;

    private bool sprinting;
    private bool jumping;
    private bool grounded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
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
        
        if (input.magnitude > 0.1f)
        {
            
            float speed = grounded && sprinting ? sprintSpeed : walkSpeed;

            Vector3 move = transform.right * input.x + transform.forward * input.y;
            transform.Translate(move * speed * Time.fixedDeltaTime, Space.World);
        }

        
        if (grounded && jumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

            //transform.Translate(Vector3.up * jumpForce * Time.fixedDeltaTime);
        }

        grounded = false; 
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
