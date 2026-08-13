using UnityEngine;

public class EnergiaPneumatica : EnergiaBase // Herda de EnergiaBase
{
    [Header("Configurações do Gás")]
    [SerializeField] private ParticleSystem particulaGas; // Arraste o Particle System no Inspector

    [Header("Animação Simples")]
    [SerializeField] private Transform volanteValvula; // O objeto que vai girar
    [SerializeField] private float anguloFechado = 90f;

    void Start()
    {
        // Define o nome do tipo para os logs automáticos do GameManager
        tipoDefinido = "Energia Pneumática";

        // Garante que as partículas comecem tocando se a energia estiver ativa
        if (_energiaAtiva && particulaGas != null)
        {
            if (!particulaGas.isPlaying) particulaGas.Play();
        }
    }

    // Sobrescreve o método padrão de desligamento da EnergiaBase
    public override void Desligar()
    {
        if (_bloqueada) return;

        if (_energiaAtiva)
        {
            _energiaAtiva = false; // Atualiza o estado lógico na EnergiaBase

            // Para as partículas de gás
            if (particulaGas != null && particulaGas.isPlaying)
            {
                particulaGas.Stop();
            }

            // Gira visualmente o volante/válvula
            if (volanteValvula != null)
            {
                volanteValvula.Rotate(0, anguloFechado, 0);
            }

            Debug.Log($"<color=orange>{nomeEnergia}:</color> {tipoDefinido} isolada e vazamento de gás interrompido!");
        }
    }

    // Sobrescreve o método de bloqueio da EnergiaBase
    public override void Bloquear()
    {
        if (!_energiaAtiva)
        {
            _bloqueada = true;
            Debug.Log($"<color=blue>{nomeEnergia}:</color> Trava física / Cadeado aplicado à válvula pneumática.");
        }
    }
}