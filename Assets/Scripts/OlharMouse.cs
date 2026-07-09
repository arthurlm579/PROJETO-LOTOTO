using UnityEngine;

public class OlharMouse : MonoBehaviour
{
    public float sensibilidadeMouse = 100f;
    public Transform corpoJogador; // Arraste o seu Player aqui no Inspector

    private float rotacaoX = 0f;

    void Start()
    {
        // Trava o mouse no centro da tela e o esconde
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse * Time.deltaTime;

        rotacaoX -= mouseY;
        rotacaoX = Mathf.Clamp(rotacaoX, -90f, 90f); // Impede de girar a cabeça de ponta-cabeça

        transform.localRotation = Quaternion.Euler(rotacaoX, 0f, 0f);
git        corpoJogador.Rotate(Vector3.up * mouseX);
    }
}