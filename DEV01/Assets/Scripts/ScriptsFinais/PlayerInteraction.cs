using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referências")]
    public Camera playerCamera;
    public PlayerController playerMovement;
    public FirstPersonHumanCamera firstPersonCamera;

    [Header("Interação")]
    public float interactionDistance = 2.5f;
    public KeyCode interactionKey = KeyCode.F;
    public LayerMask interactionLayer;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerController>();

        if (firstPersonCamera == null && playerCamera != null)
            firstPersonCamera = playerCamera.GetComponent<FirstPersonHumanCamera>();
    }

    void Update()
    {
        // Ao apertar F, tenta interagir por Raycast com objetos genéricos
        if (Input.GetKeyDown(interactionKey))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        if (playerCamera == null)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayer))
        {
            // Se o objeto acertado pelo Raycast tiver algum outro script de interação genérico no futuro,
            // você pode chamar as interações do botão F por aqui!
            Debug.Log($"[INTERAÇÃO] Raycast atingiu o objeto: {hit.collider.name}");
        }
    }
}