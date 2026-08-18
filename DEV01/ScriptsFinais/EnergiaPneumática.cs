using UnityEngine;

public class EnergiaPneumatica : EnergiaBase
{
    [Header("Configurações do Gás")]
    [SerializeField] private ParticleSystem particulaGas;

    [Header("Configurações de Áudio")]
    [Tooltip("Arraste o AudioSource com o som do ar comprimido/vazamento em loop")]
    [SerializeField] private AudioSource somArComprimido;

    [Header("Animação Simples")]
    [SerializeField] private Transform volanteValvula;
    [SerializeField] private float anguloFechado = 90f;

    void Start()
    {
        tipoDefinido = "Energia Pneumática";

        // Ativa partículas e áudio se a energia estiver ativa
        AtualizarEfeitos();
    }

    public override void Desligar()
    {
        if (_bloqueada) return;

        if (_energiaAtiva)
        {
            _energiaAtiva = false;

            // Interrompe efeitos de partículas e som
            AtualizarEfeitos();

            // Gira visualmente a válvula
            if (volanteValvula != null)
            {
                volanteValvula.Rotate(0, anguloFechado, 0);
            }

            Debug.Log($"<color=orange>{nomeEnergia}:</color> {tipoDefinido} isolada e vazamento de gás interrompido!");
        }
    }

    public override void Bloquear()
    {
        if (!_energiaAtiva)
        {
            _bloqueada = true;
            Debug.Log($"<color=blue>{nomeEnergia}:</color> Trava física / Cadeado aplicado à válvula pneumática.");
        }
    }

    private void AtualizarEfeitos()
    {
        // Partículas
        if (particulaGas != null)
        {
            if (_energiaAtiva && !particulaGas.isPlaying) particulaGas.Play();
            else if (!_energiaAtiva && particulaGas.isPlaying) particulaGas.Stop();
        }

        // Som
        if (somArComprimido != null)
        {
            if (_energiaAtiva && !somArComprimido.isPlaying) somArComprimido.Play();
            else if (!_energiaAtiva && somArComprimido.isPlaying) somArComprimido.Stop();
        }
    }
}