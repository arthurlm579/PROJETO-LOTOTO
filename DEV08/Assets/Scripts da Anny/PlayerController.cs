using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("=== Movimentação Básica ===")]
    public float walkSpeed = 6.2f;
    public float sprintSpeed = 9.8f;

    [Header("=== Pulo e Gravidade ===")]
    public float jumpHeight = 2.8f;
    public float gravity = -28f;

    [Header("=== Habilidades Avançadas ===")]
    public int maxDoubleJumps = 1;
    public float wallJumpForce = 9.5f;
    public LayerMask wallLayer;

    [Header("=== Stamina ===")]
    public float maxStamina = 100f;
    public float staminaDrainSprint = 28f;
    public float staminaRegenRate = 22f;

    // Componentes
    private CharacterController controller;
    private FirstPersonHumanCamera fpCamera;
    private PlayerStateDetector stateDetector;

    // Variáveis internas
    private Vector3 velocity;
    private bool isGrounded;
    private int doubleJumpCount = 0;
    private float currentStamina;
    private bool isSprinting = false;
    private bool isMovementLocked = false;
    private float currentSpeed01 = 0f;

    // Variável para controlar o wall jump
    private float wallJumpTimer = 0f;
    private const float wallJumpLockTime = 0.35f;

    // === PROPRIEDADES PÚBLICAS REQUERIDAS PELA CÂMERA ===
    public float CurrentSpeed01 => currentSpeed01;
    public bool IsRunning => isSprinting && currentSpeed01 > 0.1f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // 1ª Tentativa: Busca pela tag MainCamera
        if (Camera.main != null)
        {
            fpCamera = Camera.main.GetComponent<FirstPersonHumanCamera>();
        }

        // 2ª Tentativa: Se ainda for nulo, procura nos filhos deste objeto
        if (fpCamera == null)
        {
            fpCamera = GetComponentInChildren<FirstPersonHumanCamera>();
        }

        stateDetector = GetComponent<PlayerStateDetector>();
        currentStamina = maxStamina;
    }

    void Update()
    {
        if (isMovementLocked)
        {
            velocity = Vector3.zero;
            isSprinting = false;
            currentSpeed01 = 0f;
            return;
        }

        HandleGroundCheck();
        HandleStamina();
        HandleMovement();
        HandleJumpAndWallJump();

        // Aplica gravidade sempre
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (wallJumpTimer > 0)
            wallJumpTimer -= Time.deltaTime;
    }

    private void HandleGroundCheck()
    {
        isGrounded = stateDetector != null ? stateDetector.IsGroundedNow() : controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            doubleJumpCount = 0;
            wallJumpTimer = 0f;
        }
    }

    private void HandleMovement()
    {
        if (wallJumpTimer > 0) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Se mesmo após as tentativas a câmera não existir, usamos a direção do próprio Player para não travar o movimento
        Vector3 forward = fpCamera != null ? fpCamera.transform.forward : transform.forward;
        Vector3 right = fpCamera != null ? fpCamera.transform.right : transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

        currentSpeed01 = moveDirection.magnitude;

        isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 5f && currentSpeed01 > 0.1f;

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    private void HandleJumpAndWallJump()
    {
        if (!Input.GetButtonDown("Jump")) return;

        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            doubleJumpCount = 0;
        }
        else if (doubleJumpCount < maxDoubleJumps)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity) * 0.92f;
            doubleJumpCount++;
        }
        else if (stateDetector != null && stateDetector.IsOnWallNow())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity) * 1.15f;

            Vector3 awayFromWall = fpCamera != null ? -fpCamera.transform.forward : -transform.forward;
            velocity.x = awayFromWall.x * wallJumpForce;
            velocity.z = awayFromWall.z * wallJumpForce;

            wallJumpTimer = wallJumpLockTime;
            doubleJumpCount = 0;
        }
    }

    private void HandleStamina()
    {
        if (isSprinting && wallJumpTimer <= 0)
        {
            currentStamina -= staminaDrainSprint * Time.deltaTime;
            if (currentStamina < 0) currentStamina = 0;
        }
        else
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
        }
    }

    // === FUNÇÕES PÚBLICAS ===
    public void SetMovementLocked(bool locked)
    {
        isMovementLocked = locked;
    }

    public void RestoreStamina(float amount)
    {
        currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
    }

    public void Respawn(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        velocity = Vector3.zero;
        wallJumpTimer = 0f;
        controller.enabled = true;
    }
}