using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovimentoJogador : MonoBehaviour
{
    public float velocidade = 5f;
    public float gravidade = -9.81f;

    private CharacterController controller;
    private Vector3 velocidadeVertical;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Pega os inputs do teclado (WASD / Setas)
        float moverX = Input.GetAxis("Horizontal");
        float moverZ = Input.GetAxis("Vertical");

        // Calcula a direção baseado para onde o jogador está olhando
        Vector3 movimento = transform.right * moverX + transform.forward * moverZ;

        // Move o jogador
        controller.Move(movimento * velocidade * Time.deltaTime);

        // Aplica a gravidade para o jogador não flutuar
        if (controller.isGrounded && velocidadeVertical.y < 0)
        {
            velocidadeVertical.y = -2f;
        }
        velocidadeVertical.y += gravidade * Time.deltaTime;
        controller.Move(velocidadeVertical * Time.deltaTime);
    }
}