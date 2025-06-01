using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float rotationSpeed = 500f;
    public float gravity = -20f;
    public float terminalVelocity = -50f;
    public float jumpHeight = 3f;
    public float groundCheckRadius = 0.4f; // Радиус для проверки приземления
    public string groundTag = "Ground"; // Тег для объектов земли

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Animation")]
    public Animator animator;
    private const string ANIM_SPRINT = "Sprint";
    private const string ANIM_RUN_FORWARD = "RunForward";
    private const string ANIM_RUN_BACKWARD = "RunBackward";
    private const string ANIM_JUMP = "Jump";

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f; // Уменьшение выносливости в секунду при беге
    public float staminaRegenRate = 10f; // Восстановление выносливости в секунду, когда не бегаем
    public float timeToStartRegen = 2f; // Время в секундах, прежде чем начать восстановление выносливости
    public Slider staminaSlider; // Ссылка на UI Slider (полоску)
    private float currentStamina;
    private float lastRunTime; // Время последнего бега

    [Header("Respawn")]
    public float fallThreshold = -500f;
    public Vector3 respawnPosition = Vector3.zero;

    // New variable: Prevent Rotation
    public bool preventRotation = false;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina; // Инициализация выносливости
        UpdateStaminaUI();
        lastRunTime = -timeToStartRegen; //Чтобы сразу началась регенерация при старте
    }

    private void Update()
    {
        isGrounded = GroundCheckByTag();
        // Check for falling below the threshold
        if (transform.position.y < fallThreshold)
        {
            
            return;
        }

        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
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

        // Running and Stamina Check
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0; // Check Stamina here
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
            animator.SetBool(ANIM_JUMP, true);
        }

        // Apply Gravity
        if (!isGrounded)  // Применяем гравитацию только в воздухе
        {
            velocity.y += gravity * Time.deltaTime;
        }
        velocity.y = Mathf.Max(velocity.y, terminalVelocity); // Ограничиваем падение
        controller.Move(velocity * Time.deltaTime);

        // Stamina Regeneration and Drain
        if (isRunning)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            lastRunTime = Time.time; // Записываем время последнего бега
            if (currentStamina < 0)
            {
                currentStamina = 0;
                isRunning = false; // Больше не бежим, если выносливость кончилась
            }
        }

        // Регенерация, только если прошло достаточно времени после последнего бега
        if (Time.time - lastRunTime > timeToStartRegen)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina > maxStamina) currentStamina = maxStamina;
        }

        UpdateStaminaUI(); // Update Stamina UI after changes

        // Animation Handling
        UpdateAnimations(x, z, isRunning, velocity.y, isGrounded, moveDirection.magnitude);
    }

    private void UpdateAnimations(float horizontalInput, float verticalInput, bool isRunning, float verticalVelocity, bool isGrounded, float moveMagnitude)
    {
        animator.SetBool(ANIM_SPRINT, isRunning && moveMagnitude > 0);
        animator.SetBool(ANIM_RUN_FORWARD, verticalInput > 0);
        animator.SetBool(ANIM_RUN_BACKWARD, verticalInput < 0);

        if (isGrounded)
        {
            animator.SetBool(ANIM_JUMP, false);
        }
        else
        {
            animator.SetBool(ANIM_JUMP, verticalVelocity > 0);
        }
    }
    private bool GroundCheckByTag()
    {
        // Используем Physics.CheckSphere для проверки
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, groundCheckRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag(groundTag))
            {
                return true; // Нашли объект с нужным тегом
            }
        }
        return false;
    }



    private void UpdateStaminaUI()
    {
        staminaSlider.value = currentStamina / maxStamina;
    }

public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }
}