using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LiminalFirstPersonCamera : MonoBehaviour
{
    [Header("=== Configurações Básicas ===")]
    public Transform playerBody;                    // Arraste o Player aqui (o objeto com CharacterController)

    [Header("=== Sensibilidade do Mouse ===")]
    public float mouseSensitivity = 135f;

    [Header("=== Movimentos da Cabeça (Terror) ===")]
    public float headBobSpeed = 7.5f;
    public float headBobAmount = 0.085f;
    public float breathIntensity = 0.018f;
    public float breathSpeed = 1.6f;
    public float swayAmount = 0.7f;

    [Header("=== Efeitos de Terror Liminal ===")]
    public float cameraDriftIntensity = 0.008f;     // Flutuação estranha quando parado

    private float xRotation = 0f;
    private Vector3 originalCameraPosition;
    private float bobTimer = 0f;
    private float swayTimer = 0f;

    void Start()
    {
        if (playerBody == null)
            playerBody = transform.parent; // caso a câmera esteja como filho do player

        originalCameraPosition = transform.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Esconde o modelo da cabeça do player (importante em FP)
        if (playerBody != null)
        {
            Renderer[] renderers = playerBody.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                if (r.gameObject.name.ToLower().Contains("head") ||
                    r.gameObject.name.ToLower().Contains("helmet"))
                    r.enabled = false;
            }
        }
    }

    void Update()
    {
        if (playerBody == null) return;

        HandleMouseLook();
        HandleHeadMovement();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -82f, 82f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void HandleHeadMovement()
    {
        CharacterController cc = playerBody.GetComponent<CharacterController>();
        float speed = cc != null ? cc.velocity.magnitude : 0f;

        if (speed > 0.2f) // Está andando
        {
            bobTimer += Time.deltaTime * headBobSpeed;

            float bobY = Mathf.Sin(bobTimer) * headBobAmount;
            float bobX = Mathf.Cos(bobTimer * 2f) * headBobAmount * 0.35f;

            transform.localPosition = originalCameraPosition + new Vector3(bobX, bobY, 0f);
        }
        else // Parado - respiração lenta + drift liminal
        {
            bobTimer = 0f;

            // Respiração
            float breath = Mathf.Sin(Time.time * breathSpeed) * breathIntensity;

            // Drift estranho (sensação de que o espaço está "vivo")
            float driftX = Mathf.Sin(Time.time * 0.45f) * cameraDriftIntensity;
            float driftY = Mathf.Sin(Time.time * 0.72f) * cameraDriftIntensity * 0.6f;

            transform.localPosition = originalCameraPosition + new Vector3(driftX, breath + driftY, 0f);
        }

        // Sway leve (balanço horizontal sutil)
        swayTimer += Time.deltaTime * 1.3f;
        float sway = Mathf.Sin(swayTimer) * swayAmount * 0.006f;
        transform.localPosition += new Vector3(sway, 0f, 0f);
    }

    // Função pública para dar susto (você pode chamar de outros scripts)
    public void TriggerScareShake(float duration = 0.45f, float strength = 0.22f)
    {
        StartCoroutine(ShakeCoroutine(duration, strength));
    }

    private System.Collections.IEnumerator ShakeCoroutine(float duration, float strength)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;
            transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}