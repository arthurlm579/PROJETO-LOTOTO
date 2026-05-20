using UnityEngine;
using UnityEngine.UI; // Necessário para controlar imagens da UI

public class SistemaDeAlarme : MonoBehaviour
{
    [Header("Configurações de Partículas")]
    public ParticleSystem fumaca;

    [Header("Configurações de UI")]
    public Image imagemAlarme; // Arraste a imagem vermelha aqui
    public float velocidadePulso = 5f;
    public float alphaMaximo = 0.4f; // Intensidade máxima do vermelho

    [Header("Configurações do Botão")]
    public Renderer botaoRenderer;
    public Color corSegura = Color.green;

    private bool sistemaResolvido = false;

    void Start()
    {
        // Garante que o alarme comece visível
        if (imagemAlarme != null) imagemAlarme.enabled = true;
    }

    void Update()
    {
        if (!sistemaResolvido)
        {
            FazerTelaPiscar();

            // Interação para desligar
            if (Input.GetKeyDown(KeyCode.E))
            {
                ResolverRisco();
            }
        }
    }

    void FazerTelaPiscar()
    {
        if (imagemAlarme != null)
        {
            // Usa a função Seno para criar um valor que vai e volta entre 0 e 1
            float alpha = (Mathf.Sin(Time.time * velocidadePulso) + 1f) / 2f;

            // Aplica o alpha limitado pelo valor máximo que você escolher
            Color novaCor = imagemAlarme.color;
            novaCor.a = alpha * alphaMaximo;
            imagemAlarme.color = novaCor;
        }
    }

    void ResolverRisco()
    {
        sistemaResolvido = true;

        // Para a fumaça
        if (fumaca != null) fumaca.Stop();

        // Muda a cor do botão
        if (botaoRenderer != null) botaoRenderer.material.color = corSegura;

        // Desliga o painel de alarme da tela
        if (imagemAlarme != null) imagemAlarme.enabled = false;

        Debug.Log("Alarme desligado e risco removido!");
    }
}