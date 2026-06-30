using UnityEngine;

public class FirstPersonHumanCamera : MonoBehaviour
{
    [Header("Referências")]
    public Transform playerBody;
    public PlayerMovementHuman playerMovement;

    [Header("Mouse")]
    public float mouseSensitivity = 120f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    [Header("Head Bob - Caminhada")]
    public float walkBobFrequency = 7.5f;
    public float walkBobAmount = 0.045f;

    [Header("Head Bob - Corrida")]
    public float runBobFrequency = 11.5f;
    public float runBobAmount = 0.075f;

    [Header("Respiração")]
    public float breathingFrequency = 1.2f;
    public float breathingAmount = 0.018f;

    [Header("Inclinação Humana")]
    public float sideTiltAmount = 2.2f;
    public float forwardTiltAmount = 1.1f;
    public float tiltSmoothness = 8f;

    [Header("Suavização")]
    public float positionSmoothness = 10f;
    public float rotationSmoothness = 12f;

    private float xRotation;
    private float bobTimer;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;

    private Vector3 targetLocalPosition;
    private Quaternion targetLocalRotation;

    private bool cameraLocked;
    private Transform focusTarget;
    private float focusSmoothness = 8f;

    void Start()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;

        targetLocalPosition = initialLocalPosition;
        targetLocalRotation = initialLocalRotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerMovement == null && playerBody != null)
        {
            playerMovement = playerBody.GetComponent<PlayerMovementHuman>();
        }
    }

    void Update()
    {
        if (cameraLocked)
        {
            HandleLockedCameraFocus();
            return;
        }

        HandleMouseLook();
        HandleHumanCameraMotion();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    void HandleHumanCameraMotion()
    {
        float movementAmount = 0f;
        bool running = false;

        if (playerMovement != null)
        {
            movementAmount = playerMovement.CurrentSpeed01;
            running = playerMovement.IsRunning;
        }

        float bobFrequency = running ? runBobFrequency : walkBobFrequency;
        float bobAmount = running ? runBobAmount : walkBobAmount;

        if (movementAmount > 0.05f)
        {
            bobTimer += Time.deltaTime * bobFrequency;

            float bobX = Mathf.Sin(bobTimer * 0.5f) * bobAmount * 0.55f;
            float bobY = Mathf.Abs(Mathf.Sin(bobTimer)) * bobAmount;

            targetLocalPosition = initialLocalPosition + new Vector3(bobX, bobY, 0f) * movementAmount;
        }
        else
        {
            bobTimer = Mathf.Lerp(bobTimer, 0f, Time.deltaTime * 4f);

            float breathY = Mathf.Sin(Time.time * breathingFrequency) * breathingAmount;
            targetLocalPosition = initialLocalPosition + new Vector3(0f, breathY, 0f);
        }

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        float sideTilt = -horizontalInput * sideTiltAmount;
        float forwardTilt = verticalInput * forwardTiltAmount;

        Quaternion tiltRotation = Quaternion.Euler(
            xRotation + forwardTilt,
            0f,
            sideTilt
        );

        targetLocalRotation = tiltRotation;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetLocalPosition,
            positionSmoothness * Time.deltaTime
        );

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            targetLocalRotation,
            rotationSmoothness * Time.deltaTime
        );
    }

    void HandleLockedCameraFocus()
    {
        if (focusTarget == null)
            return;

        Vector3 direction = focusTarget.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            focusSmoothness * Time.deltaTime
        );
    }

    public void LockCameraOnTarget(Transform target)
    {
        cameraLocked = true;
        focusTarget = target;
    }

    public void UnlockCamera()
    {
        cameraLocked = false;
        focusTarget = null;

        Vector3 currentEuler = transform.localEulerAngles;
        xRotation = currentEuler.x;

        if (xRotation > 180f)
            xRotation -= 360f;
    }

    public bool IsCameraLocked()
    {
        return cameraLocked;
    }
}