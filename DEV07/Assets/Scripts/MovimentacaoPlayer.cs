using UnityEngine;

public class MovimentacaoPlayer : MonoBehaviour
{
    public CharacterController controller;
    public float velocidade = 12f;
    public float gravidade = -9.81f;
    public float alturaPulo = 3f;

    Vector3 velocidadeVertical;
    bool estaNoChao;

    void Update()
    {
        // Verifica se o player está tocando o chão
        estaNoChao = controller.isGrounded;

        if (estaNoChao && velocidadeVertical.y < 0)
        {
            velocidadeVertical.y = -2f; // Mantém o player colado no chão
        }

        // Pega os inputs (W, A, S, D / Setas)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calcula a direção baseada na rotação do player
        Vector3 mover = transform.right * x + transform.forward * z;

        // Aplica o movimento
        controller.Move(mover * velocidade * Time.deltaTime);

        // Lógica de Pulo
        if (Input.GetButtonDown("Jump") && estaNoChao)
        {
            velocidadeVertical.y = Mathf.Sqrt(alturaPulo * -2f * gravidade);
        }

        // Aplica gravidade
        velocidadeVertical.y += gravidade * Time.deltaTime;
        controller.Move(velocidadeVertical * Time.deltaTime);
    }
}