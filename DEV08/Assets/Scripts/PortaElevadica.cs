using UnityEngine;

public class PortaElevadica : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [Tooltip("Altura em metros que a porta vai subir")]
    public float alturaSubida = 4.0f;
    public float velocidade = 3.0f;

    private Vector3 posicaoFechada;
    private Vector3 posicaoAberta;
    private bool estaAberta = false;

    void Start()
    {
        posicaoFechada = transform.position;
        posicaoAberta = posicaoFechada + Vector3.up * alturaSubida;
    }

    void Update()
    {
        // Move suavemente até a posição alvo
        Vector3 posicaoAlvo = estaAberta ? posicaoAberta : posicaoFechada;
        transform.position = Vector3.MoveTowards(transform.position, posicaoAlvo, velocidade * Time.deltaTime);
    }

    public void AlternarPorta()
    {
        estaAberta = !estaAberta;
    }

    public void Abrir()
    {
        estaAberta = true;
    }

    public void Fechar()
    {
        estaAberta = false;
    }
}