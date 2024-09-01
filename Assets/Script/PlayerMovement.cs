using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Unity.Mathematics;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float sprintSpeedStamina;
    public bool canMove;
    [SerializeField] float groundDrag;
    [SerializeField] Transform orientation;
    public bool stoppSpining = false;
    [SerializeField] Transform mainCam;
    private float horizontalInput;
    private float verticalInput;
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform playerObj;
    private Vector3 moveDirection;
    private Rigidbody rb;
    

    [Header("jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    [SerializeField] float airMultiplier;
    private bool readyToJump;

    [SerializeField] PlayerCombat playerCombat;
    [SerializeField] PlayerStats PlayerStats;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    [SerializeField] float playerHeight;
    [SerializeField] LayerMask whatIsGround;
    private bool grounded;

    [Header("animashens")]
    [SerializeField] Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        canMove = true;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // rotate orientation
        Vector3 viewDir = this.transform.position - new Vector3(mainCam.position.x, this.transform.position.y, mainCam.position.z);
        orientation.forward = viewDir.normalized;

        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (!stoppSpining)
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        var dir = orientation.forward.normalized;
        var animDir = transform.InverseTransformDirection(dir);
        var isFacingMoveDirection = Vector3.Dot(dir, orientation.forward) > .5f;

/*        animator.SetFloat("Horizontal", isFacingMoveDirection ? rb.velocity.x : 0, .1f, Time.deltaTime);
        animator.SetFloat("Vertical", isFacingMoveDirection ? rb.velocity.z : 0, .1f, Time.deltaTime);*/
        animator.SetFloat("velocity", isFacingMoveDirection ? (rb.velocity.magnitude / sprintSpeed) : 0, .1f, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        if (canMove)
        {
            // calculate movement direction
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

            float speed = walkSpeed;
            if (Input.GetKey(sprintKey) && PlayerStats.Stamina > sprintSpeedStamina)
            {
                speed = sprintSpeed;
                PlayerStats.Stamina -= sprintSpeedStamina;
                PlayerStats.UpdateHealthUI();
            }

            // on ground
            if (grounded)
                rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);

            // in air
            else if (!grounded)
                rb.AddForce(moveDirection.normalized * speed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > walkSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * walkSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        animator.CrossFade("jump", .2f);
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}