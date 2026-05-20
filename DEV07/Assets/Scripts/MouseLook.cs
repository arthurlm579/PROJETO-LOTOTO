using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Configuracoes de Sensibilidade")]
    public float sensibilidade = 100f;

    [Header("Referencias")]
    public Transform corpo; // Arraste o objeto pai (Player) aqui

    private float rotacaoX = 0f;

    void Start()
    {
        // Garante que o mouse fique travado no centro ao iniciar
        // Útil para evitar que o cursor saia da janela do jogo
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // Se o jogo estiver pausado (Time.timeScale = 0), não rotaciona a câmera
        if (Time.timeScale == 0f) return;

        // Captura o movimento do mouse
        // Multiplicamos por Time.deltaTime para que a velocidade seja independente do FPS
        float mouseX = Input.GetAxis("Mouse X") * sensibilidade * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidade * Time.deltaTime;

        // 1. Controle da rotação vertical (Olhar para cima e para baixo)
        rotacaoX -= mouseY;

        // Clamp: Limita a rotação para o jogador não dar uma cambalhota com a cabeça
        rotacaoX = Mathf.Clamp(rotacaoX, -80f, 80f);

        // Aplica a rotação apenas no eixo X da câmera
        transform.localRotation = Quaternion.Euler(rotacaoX, 0f, 0f);

        // 2. Controle da rotação horizontal (Girar o corpo do jogador)
        if (corpo != null)
        {
            corpo.Rotate(Vector3.up * mouseX);
        }
    }
}