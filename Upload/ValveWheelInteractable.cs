using UnityEngine;

public class ValveWheelInteractable : MonoBehaviour
{
    [Header("Referências")]
    public Transform wheelTransform;
    public Transform cameraFocusPoint;

    [Header("Controle")]
    public KeyCode rotateLeftKey = KeyCode.Q;
    public KeyCode rotateRightKey = KeyCode.E;

    [Header("Controle Alternativo")]
    public bool allowADKeys = true;

    [Header("Rotação")]
    public float rotationSpeed = 95f;

    [Tooltip("Eixo local da roda. Para a roda que a gente criou antes, normalmente é Y.")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Limites da Válvula")]
    public bool useRotationLimit = true;
    public float minRotation = 0f;
    public float maxRotation = 360f;

    [Header("Estado")]
    public bool startsOpen = false;

    private PlayerInteraction currentUser;
    private bool beingUsed;

    private float currentRotation;

    private Quaternion initialWheelRotation;

    public float OpenPercent
    {
        get
        {
            if (!useRotationLimit)
                return 0f;

            return Mathf.InverseLerp(minRotation, maxRotation, currentRotation);
        }
    }

    void Start()
    {
        if (wheelTransform == null)
            wheelTransform = transform;

        if (cameraFocusPoint == null)
            cameraFocusPoint = transform;

        initialWheelRotation = wheelTransform.localRotation;

        currentRotation = startsOpen ? maxRotation : minRotation;
        ApplyWheelRotation();
    }

    void Update()
    {
        if (!beingUsed)
            return;

        HandleValveRotation();
    }

    void HandleValveRotation()
    {
        float input = 0f;

        if (Input.GetKey(rotateLeftKey))
            input -= 1f;

        if (Input.GetKey(rotateRightKey))
            input += 1f;

        if (allowADKeys)
        {
            if (Input.GetKey(KeyCode.A))
                input -= 1f;

            if (Input.GetKey(KeyCode.D))
                input += 1f;
        }

        if (Mathf.Abs(input) < 0.1f)
        {
            ApplyWheelRotation();
            return;
        }

        currentRotation += input * rotationSpeed * Time.deltaTime;

        if (useRotationLimit)
        {
            currentRotation = Mathf.Clamp(currentRotation, minRotation, maxRotation);
        }

        ApplyWheelRotation();
    }

    void ApplyWheelRotation()
    {
        if (wheelTransform == null)
            return;

        Quaternion spinRotation = Quaternion.AngleAxis(
            currentRotation,
            rotationAxis.normalized
        );

        wheelTransform.localRotation = initialWheelRotation * spinRotation;
    }

    public void StartInteraction(PlayerInteraction user)
    {
        currentUser = user;
        beingUsed = true;
        ApplyWheelRotation();
    }

    public void StopInteraction()
    {
        beingUsed = false;
        currentUser = null;
        ApplyWheelRotation();
    }

    public bool IsBeingUsed()
    {
        return beingUsed;
    }
}