using UnityEngine;

public class ValvulaGas : MonoBehaviour
{
    [Header("Configurações do Gás")]
    public ParticleSystem particulaGas; // Arraste o seu Particle System aqui no Inspector
    public bool gasVazando = true;

    [Header("Animação Simples")]
    public Transform volanteValvula; // O objeto que vai girar (pode ser a própria válvula)
    public float anguloFechado = 90f;

    void Start()
    {
        // Garante que o gás comece tocando se estiver vazando
        if (gasVazando && particulaGas != null)
        {
            particulaGas.Play();
        }
    }

    public void Interagir()
    {
        if (gasVazando)
        {
            FecharValvula();
        }
    }

    void FecharValvula()
    {
        gasVazando = false;

        // Para as partículas de gás
        if (particulaGas != null)
        {
            particulaGas.Stop();
        }

        // Gira visualmente a válvula para indicar que foi fechada
        if (volanteValvula != null)
        {
            volanteValvula.Rotate(0, anguloFechado, 0);
        }

        Debug.Log("Válvula fechada com sucesso! Gás interrompido.");
    }
}