using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float rotationSpeed = 500f;
    public float gravity = -9.81f;
    public float terminalVelocity = -50f; // Maximum falling speed
    public float jumpHeight = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Animation")]
    public Animator animator;
    private const string ANIM_SPRINT = "Sprint";
    private const string ANIM_RUN_FORWARD = "RunForward";
    private const string ANIM_JUMP = "Jump";

    // New variable: Prevent Rotation
    public bool preventRotation = false; // Set this to true in the Inspector

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        // Check if CharacterController exists.
        if (controller == null)
        {
            Debug.LogError("CharacterController not found on this GameObject.");
            enabled = false;
        }

        // Check if Animator exists
        if (animator == null)
        {
            Debug.LogError("Animator component not found. Please assign it in the Inspector.");
            enabled = false;
        }
    }

    private void Update()
    {
        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }

        // Input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Determine movement direction
        Vector3 moveDirection = transform.forward * z + transform.right * x;

        // Normalize to prevent faster diagonal movement
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // Running
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Apply movement
        Vector3 move = moveDirection * currentSpeed * Time.deltaTime;
        controller.Move(move);

        // Rotation
        if (!preventRotation && moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * -gravity);
            animator.SetBool(ANIM_JUMP, true); // Start the jump animation
        }

        // Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, terminalVelocity); // Clamp the falling speed

        controller.Move(velocity * Time.deltaTime);

        // Animation Handling
        UpdateAnimations(x, z, isRunning, velocity.y, isGrounded);
    }

    // Optional: Draw a gizmo to visualize the ground check radius
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }
    private void UpdateAnimations(float horizontalInput, float verticalInput, bool isRunning, float verticalVelocity, bool isGrounded)
    {
        //Running animations
        animator.SetBool(ANIM_SPRINT, isRunning && (verticalInput > 0 || verticalInput < 0)); //Sprint when moving forward or backwards

        //Forward Running
        animator.SetBool(ANIM_RUN_FORWARD, verticalInput > 0 );

        //Jump Animation
        if (isGrounded)
        {
            animator.SetBool(ANIM_JUMP, false); // Ensure Jump is false when grounded
        }
        else
        {
            animator.SetBool(ANIM_JUMP, verticalVelocity > 0); // Only set to true when rising
        }

    }
}