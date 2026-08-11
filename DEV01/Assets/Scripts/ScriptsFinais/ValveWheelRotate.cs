using UnityEngine;

public class ValveWheelRotate : MonoBehaviour
{
    [Header("Configurações de Rotação")]
    [Tooltip("Velocidade com que o objeto vai girar.")]
    public float rotationSpeed = 80f;

    [Tooltip("Direção/Eixo em que o objeto vai girar (ex: Vector3.up para Y, Vector3.forward para Z).")]
    public Vector3 rotationAxis = Vector3.up;

    [Header("Controle de Input")]
    [Tooltip("Se marcado, gira apenas ao pressionar as teclas. Se desmarcado, gira continuamente sozinho.")]
    public bool rotateWithInput = true;

    [Tooltip("Tecla para girar no sentido positivo do eixo.")]
    public KeyCode rotatePositiveKey = KeyCode.E;

    [Tooltip("Tecla para girar no sentido negativo do eixo.")]
    public KeyCode rotateNegativeKey = KeyCode.Q;

    void Update()
    {
        // Garante que o vetor do eixo esteja normalizado (tamanho 1) para não afetar a velocidade
        Vector3 axis = rotationAxis.normalized;

        if (rotateWithInput)
        {
            if (Input.GetKey(rotatePositiveKey))
            {
                transform.Rotate(axis * rotationSpeed * Time.deltaTime, Space.Self);
            }

            if (Input.GetKey(rotateNegativeKey))
            {
                transform.Rotate(-axis * rotationSpeed * Time.deltaTime, Space.Self);
            }
        }
        else
        {
            // Rotação contínua automática no eixo escolhido
            transform.Rotate(axis * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}