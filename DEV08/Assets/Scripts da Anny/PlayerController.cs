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
    public float wallJumpForce = 9.5f;          // Força horizontal do wall jump
    public LayerMask wallLayer;

    [Header("=== Stamina ===")]
    public float maxStamina = 100f;
    public float staminaDrainSprint = 28f;
    public float staminaRegenRate = 22f;

    // Componentes
    private CharacterController controller;
    private LiminalFirstPersonCamera fpCamera;
    private PlayerStateDetector stateDetector;   // Usando o detector que você pediu

    // Variáveis internas
    private Vector3 velocity;
    private bool isGrounded;
    private int doubleJumpCount = 0;
    private float currentStamina;
    private bool isSprinting = false;

    // Variável para controlar o wall jump (evita conflito infinito)
    private float wallJumpTimer = 0f;
    private const float wallJumpLockTime = 0.35f;   // Tempo que o player fica "travado" no impulso (ajuste se precisar)

    void Start()
    {
        controller = GetComponent<CharacterController>();
        fpCamera = Camera.main.GetComponent<LiminalFirstPersonCamera>();
        stateDetector = GetComponent<PlayerStateDetector>();

        currentStamina = maxStamina;
    }

    void Update()
    {
        HandleGroundCheck();
        HandleStamina();
        HandleMovement();
        HandleJumpAndWallJump();

        // Aplica gravidade sempre
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Timer do wall jump (diminui o lock)
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
            wallJumpTimer = 0f;           // Reseta o lock ao tocar o chão
        }
    }

    private void HandleMovement()
    {
        // Durante o wall jump, não permite input horizontal (evita conflito)
        if (wallJumpTimer > 0) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = fpCamera.transform.forward;
        Vector3 right = fpCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

        isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 5f && moveDirection.magnitude > 0.1f;

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    private void HandleJumpAndWallJump()
    {
        if (!Input.GetButtonDown("Jump")) return;

        if (isGrounded)
        {
            // Pulo normal
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            doubleJumpCount = 0;
        }
        else if (doubleJumpCount < maxDoubleJumps)
        {
            // Double Jump
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity) * 0.92f;
            doubleJumpCount++;
        }
        else if (stateDetector != null && stateDetector.IsOnWallNow())
        {
            // WALL JUMP - Versão corrigida
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity) * 1.15f;

            // Empurra forte para longe da parede (usando direção da câmera)
            Vector3 awayFromWall = -fpCamera.transform.forward;
            velocity.x = awayFromWall.x * wallJumpForce;
            velocity.z = awayFromWall.z * wallJumpForce;

            wallJumpTimer = wallJumpLockTime;   // Ativa o lock para não conflitar com input
            doubleJumpCount = 0;                // Reseta double jump após wall jump
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

    // Funções públicas
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