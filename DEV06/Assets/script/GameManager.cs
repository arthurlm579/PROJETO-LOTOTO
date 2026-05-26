using UnityEngine;
using System.Collections.Generic; // Necessário para usar List

public class GerenciadorEnergia : MonoBehaviour
{
    [Header("Configurações de Energia")]
    // Lista que vai conter todos os objetos com o script EnergiaHidraulica
    public List<EnergiaHidraulica> energias = new List<EnergiaHidraulica>();

    // Tempo que cada energia vai demorar para desligar após o comando
    public float tempoDeAtraso = 2.0f;

    [Header("Feedback Visual e Sonoro")]
    // Arraste o seu componente de AudioSource do alarme para cá no Inspector
    public AudioSource alarme;

    // FUNÇÃO PARA O BOTÃO: Desliga tudo com delay
    public void ComandoDesligarGeral()
    {
        foreach (EnergiaHidraulica e in energias)
        {
            if (e != null) // Proteção para evitar erros caso algum item da lista esteja vazio
            {
                e.IniciarDesligamento(tempoDeAtraso);
            }
        }
    }

    // Função que verifica se é seguro entrar na zona (chamada pela ZonaDeRisco)
    public void VerificarEstado()
    {
        foreach (EnergiaHidraulica e in energias)
        {
            if (e != null && e.energiaAtiva)
            {
                DispararErro();
                return; // Para o loop na primeira energia ativa que encontrar
            }
        }
        Sucesso();
    }

    void DispararErro()
    {
        Debug.LogWarning("ERRO: Ainda existem energias ativas!");
        if (alarme != null && !alarme.isPlaying)
        {
            alarme.Play();
        }
    }

    void Sucesso()
    {
        Debug.Log("SUCESSO: Sistema seguro! Energia zerada.");
        if (alarme != null && alarme.isPlaying)
        {
            alarme.Stop(); // Opcional: para o alarme se o jogador resolver o problema
        }
    }
}