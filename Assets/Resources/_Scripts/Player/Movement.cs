using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    [Header("Components")]
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public PhotonView photonView;
    private Animator anim;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float airControlMultiplier = 0.5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;

    [Header("Tuning")]
    [SerializeField, Tooltip("Velocidad de respuesta de aceleración")]
    private float accel = 10f;

    private Vector2 input;
    private bool jumping;
    private bool sprinting;
    private bool grounded;

    private float baseWalkSpeed;
    private float baseSprintSpeed;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        rb.freezeRotation = true;

        baseWalkSpeed = walkSpeed;
        baseSprintSpeed = sprintSpeed;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        sprinting = Input.GetButton("Sprint");
        jumping = Input.GetButton("Jump");

        if (groundCheck != null)
            grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        anim.SetBool("Running", sprinting);
        //anim.SetBool("Jumping", jumping);

        //if (Input.GetKeyDown(KeyCode.M)) anim.SetTrigger("Dance");
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        HandleMovement();
        HandleJump();

        anim.SetBool("Moving", input.magnitude > 0.1f);
    }

    private void HandleMovement()
    {
        Vector3 moveDir = (transform.right * input.x + transform.forward * input.y).normalized;

        float targetSpeed = (sprinting && grounded) ? sprintSpeed : walkSpeed;
        if (!grounded)
            targetSpeed *= airControlMultiplier;

        Vector3 desiredHorizontal = moveDir * targetSpeed;

        Vector3 currentVel = rb.velocity;
        Vector3 currentHorizontal = new Vector3(currentVel.x, 0f, currentVel.z);

        float blend = Mathf.Clamp01(accel * Time.fixedDeltaTime);
        Vector3 newHorizontal = Vector3.Lerp(currentHorizontal, desiredHorizontal, blend);

        Vector3 newVelocity = new Vector3(newHorizontal.x, currentVel.y, newHorizontal.z);
        rb.velocity = newVelocity;
    }

    private void HandleJump()
    {
        if (jumping && grounded)
        {
            Vector3 v = rb.velocity;
            v.y = jumpForce;
            rb.velocity = v;

            anim.SetTrigger("Jumping");
        }
            jumping = false;
    }

    public void SpeedBoost(float multiplier)
    {
        float maxWalk = baseWalkSpeed * 2f;
        float maxSprint = baseSprintSpeed * 2f;

        walkSpeed = Mathf.Min(walkSpeed * multiplier, maxWalk);
        sprintSpeed = Mathf.Min(sprintSpeed * multiplier, maxSprint);
    }

    //public void OnMoveInput(Vector2 inputValue) => input = inputValue;
    //public void OnJumpInput(bool value) => jumping = value;
    //public void OnSprintInput(bool value) => sprinting = value;

    private void OnTriggerStay(Collider other)
    {
        grounded = true;
    }
}
