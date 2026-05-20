using UnityEngine;

public class PlayerStateDetector : MonoBehaviour
{
    [Header("=== Configurações de Detecção ===")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    [Header("=== Distâncias de Raycast ===")]
    public float groundCheckDistance = 0.15f;      // Distância para detectar o chão
    public float wallCheckDistance = 0.85f;        // Distância para detectar parede à frente
    public float wallCheckHeightOffset = 0.8f;     // Altura onde começa o raycast da parede

    [Header("=== Debug (deixe ligado para ver no Scene) ===")]
    public bool showDebugRays = true;

    // Estados públicos (você pode ler esses bools de qualquer outro script)
    public bool IsGrounded { get; private set; }
    public bool IsOnWall { get; private set; }
    public bool IsInAir { get; private set; }
    public bool IsTouchingWallAndGround { get; private set; }

    // Referência ao CharacterController
    private CharacterController controller;
    private Vector3 lastPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastPosition = transform.position;
    }

    void Update()
    {
        DetectGround();
        DetectWall();
        UpdateCombinedStates();
        DrawDebugRays();
    }

    private void DetectGround()
    {
        // Usa o próprio CharacterController + Raycast extra para mais precisão
        bool ccGrounded = controller.isGrounded;

        // Raycast para baixo (mais confiável em plataformas móveis)
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        IsGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundLayer) || ccGrounded;
    }

    private void DetectWall()
    {
        // Raycast à frente do player (na direção que a câmera está olhando)
        Vector3 origin = transform.position + Vector3.up * wallCheckHeightOffset;
        Vector3 direction = Camera.main.transform.forward;   // Usa a direção da câmera FP

        IsOnWall = Physics.Raycast(origin, direction, wallCheckDistance, wallLayer);
    }

    private void UpdateCombinedStates()
    {
        IsInAir = !IsGrounded && !IsOnWall;

        // Estado raro: tocando chão e parede ao mesmo tempo (canto)
        IsTouchingWallAndGround = IsGrounded && IsOnWall;
    }

    private void DrawDebugRays()
    {
        if (!showDebugRays) return;

        // Ray do chão (verde quando tocando, vermelho quando não)
        Vector3 groundOrigin = transform.position + Vector3.up * 0.1f;
        Debug.DrawRay(groundOrigin, Vector3.down * groundCheckDistance, IsGrounded ? Color.green : Color.red);

        // Ray da parede (azul quando tocando, vermelho quando não)
        Vector3 wallOrigin = transform.position + Vector3.up * wallCheckHeightOffset;
        Vector3 wallDirection = Camera.main.transform.forward;
        Debug.DrawRay(wallOrigin, wallDirection * wallCheckDistance, IsOnWall ? Color.cyan : Color.red);
    }

    // ====================== Funções Públicas para outros scripts ======================

    public bool IsGroundedNow() => IsGrounded;
    public bool IsOnWallNow() => IsOnWall;
    public bool IsInAirNow() => IsInAir;
    public bool IsTouchingCorner() => IsTouchingWallAndGround;

    // Retorna uma string com o estado atual (útil para debug ou para decidir animações/sons)
    public string GetCurrentStateName()
    {
        if (IsTouchingWallAndGround) return "Corner (Chão + Parede)";
        if (IsGrounded) return "Grounded (No Chão)";
        if (IsOnWall) return "On Wall (Na Parede)";
        if (IsInAir) return "In Air (No Ar)";
        return "Unknown";
    }
}