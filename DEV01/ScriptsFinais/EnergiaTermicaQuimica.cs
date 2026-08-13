using UnityEngine;
using UnityEngine.UI;

public class EnergiaTermicaQuimica : EnergiaBase
{
    [Header("Configurações de Partículas")]
    [SerializeField] private ParticleSystem fumaca;

    [Header("Configurações de UI")]
    [SerializeField] private Image imagemAlarme; // Arraste a imagem vermelha aqui
    [SerializeField] private float velocidadepulso = 5f;
    [SerializeField] private float alphaMaximo = 0.4f; // Intensidade máxima do vermelho

    [Header("Configurações do Botão/Painel")]
    [SerializeField] private Renderer botaoRenderer;
    [SerializeField] private Color corSegura = Color.green;

    void Start()
    {
        // Define o nome do tipo para os logs automáticos do GameManager
        tipoDefinido = "Energia Térmica / Química";

        // Garante que o alarme visual comece ativo se a energia estiver ligada
        if (imagemAlarme != null)
        {
            imagemAlarme.enabled = _energiaAtiva;
        }

        // NOVIDADE: Verifica o estado inicial das partículas ao começar o jogo
        AtualizarParticulas();
    }

    void Update()
    {
        // Enquanto a energia estiver ativa (não resolvida), mantém o efeito de piscar na tela
        AtualizarParticulas();

        if (_energiaAtiva)
        {
            FazerTelaPiscar();
        }
    }

    private void FazerTelaPiscar()
    {
        if (imagemAlarme != null)
        {
            float alpha = (Mathf.Sin(Time.time * velocidadepulso) + 1f) / 2f;
            Color novaCor = imagemAlarme.color;
            novaCor.a = alpha * alphaMaximo;
            imagemAlarme.color = novaCor;
        }
    }

    // Sobrescreve o método padrão de desligamento da EnergiaBase
    public override void Desligar()
    {
        if (_bloqueada) return;

        if (_energiaAtiva)
        {
            _energiaAtiva = false; // Atualiza o estado lógico na EnergiaBase

            // Chamamos a função para parar as partículas imediatamente
            AtualizarParticulas();

            // Muda a cor do botão físico para verde (seguro)
            if (botaoRenderer != null) botaoRenderer.material.color = corSegura;

            // Desliga a overlay vermelha piscando na tela do jogador
            if (imagemAlarme != null) imagemAlarme.enabled = false;

            Debug.Log($"<color=orange>{nomeEnergia}</color> {tipoDefinido} isolada e vazamento interrompido!");
        }
    }

    // Sobrescreve o método de bloqueio para o padrão do console
    public override void Bloquear()
    {
        if (!_energiaAtiva)
        {
            _bloqueada = true;
            Debug.Log($"<color=blue>{nomeEnergia}</color> Cadeado de bloqueio aplicado à válvula química.");
        }
    }

    // NOVIDADE: Função dedicada a controlar o ciclo de vida do Particle System
    private void AtualizarParticulas()
    {
        if (fumaca != null)
        {
            if (_energiaAtiva)
            {
                // Se a energia está ativa, o gás/fumaça deve sair
                if (!fumaca.isPlaying) fumaca.Play();
            }
            else
            {
                // Se a energia foi desligada, a fumaça para de sair imediatamente
                if (fumaca.isPlaying) fumaca.Stop();
            }
        }
    }
}