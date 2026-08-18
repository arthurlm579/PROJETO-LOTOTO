using UnityEngine;
using UnityEngine.UI;

public class EnergiaTermicaQuimica : EnergiaBase
{
    [Header("Configurações de Partículas")]
    [SerializeField] private ParticleSystem fumaca;

    [Header("Configurações de Áudio")]
    [Tooltip("Arraste o AudioSource com o som do vazamento/vapor em loop")]
    [SerializeField] private AudioSource somVazamento;

    [Header("Configurações de UI")]
    [SerializeField] private Image imagemAlarme;
    [SerializeField] private float velocidadepulso = 5f;
    [SerializeField] private float alphaMaximo = 0.4f;

    [Header("Configurações do Botão/Painel")]
    [SerializeField] private Renderer botaoRenderer;
    [SerializeField] private Color corSegura = Color.green;

    void Start()
    {
        tipoDefinido = "Energia Térmica / Química";

        if (imagemAlarme != null)
        {
            imagemAlarme.enabled = _energiaAtiva;
        }

        AtualizarSonsEParticulas();
    }

    void Update()
    {
        AtualizarSonsEParticulas();

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

    public override void Desligar()
    {
        if (_bloqueada) return;

        if (_energiaAtiva)
        {
            _energiaAtiva = false;

            AtualizarSonsEParticulas();

            if (botaoRenderer != null) botaoRenderer.material.color = corSegura;
            if (imagemAlarme != null) imagemAlarme.enabled = false;

            Debug.Log($"<color=orange>{nomeEnergia}</color> {tipoDefinido} isolada e vazamento interrompido!");
        }
    }

    public override void Bloquear()
    {
        if (!_energiaAtiva)
        {
            _bloqueada = true;
            Debug.Log($"<color=blue>{nomeEnergia}</color> Cadeado de bloqueio aplicado à válvula química.");
        }
    }

    private void AtualizarSonsEParticulas()
    {
        // Controle de Partículas
        if (fumaca != null)
        {
            if (_energiaAtiva)
            {
                if (!fumaca.isPlaying) fumaca.Play();
            }
            else
            {
                if (fumaca.isPlaying) fumaca.Stop();
            }
        }

        // Controle de Áudio (Novo)
        if (somVazamento != null)
        {
            if (_energiaAtiva)
            {
                if (!somVazamento.isPlaying) somVazamento.Play();
            }
            else
            {
                if (somVazamento.isPlaying) somVazamento.Stop();
            }
        }
    }
}