using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referências")]
    public Camera playerCamera;
    public PlayerMovementHuman playerMovement;
    public FirstPersonHumanCamera firstPersonCamera;

    [Header("Interação")]
    public float interactionDistance = 2.5f;
    public KeyCode interactionKey = KeyCode.F;
    public LayerMask interactionLayer;

    private ValveWheelInteractable currentValve;

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovementHuman>();

        if (firstPersonCamera == null && playerCamera != null)
            firstPersonCamera = playerCamera.GetComponent<FirstPersonHumanCamera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            if (currentValve != null && currentValve.IsBeingUsed())
            {
                StopUsingValve();
                return;
            }

            TryInteract();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentValve != null && currentValve.IsBeingUsed())
            {
                StopUsingValve();
            }
        }
    }

    void TryInteract()
    {
        if (playerCamera == null)
            return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayer))
        {
            ValveWheelInteractable valve = hit.collider.GetComponentInParent<ValveWheelInteractable>();

            if (valve != null)
            {
                StartUsingValve(valve);
            }
        }
    }

    void StartUsingValve(ValveWheelInteractable valve)
    {
        currentValve = valve;
        currentValve.StartInteraction(this);

        if (playerMovement != null)
            playerMovement.SetMovementLocked(true);

        if (firstPersonCamera != null)
            firstPersonCamera.LockCameraOnTarget(currentValve.cameraFocusPoint);
    }

    public void StopUsingValve()
    {
        if (currentValve != null)
        {
            currentValve.StopInteraction();
            currentValve = null;
        }

        if (playerMovement != null)
            playerMovement.SetMovementLocked(false);

        if (firstPersonCamera != null)
            firstPersonCamera.UnlockCamera();
    }
}