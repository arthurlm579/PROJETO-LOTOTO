using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // 1. Estrutura de estados
    public enum EstadoDoJogo { Iniciando, EmJogo, Erro, Sucesso, Pausado }

    [Header("Configurações de Fluxo")]
    [SerializeField] private EstadoDoJogo estadoAtual = EstadoDoJogo.Iniciando;

    [Header("Referências")]
    [SerializeField] private List<Energia> listaDeEnergias;
    [SerializeField] private AudioSource somAlarme;

    void Start()
    {
        // Inicia o fluxo
        AlterarEstado(EstadoDoJogo.EmJogo);
    }

    // 2. Controle de fluxo centralizado
    public void AlterarEstado(EstadoDoJogo novoEstado)
    {
        // 5. Bloqueio do sistema: Impede mudanças se o jogo já terminou
        if (estadoAtual == EstadoDoJogo.Sucesso || estadoAtual == EstadoDoJogo.Erro)
        {
            Debug.LogWarning($"Troca negada: O jogo já finalizou em {estadoAtual}.");
            return;
        }

        // Registra a transição no console
        Debug.Log($"<color=white>Transição de Estado:</color> {estadoAtual} -> <b>{novoEstado}</b>");

        estadoAtual = novoEstado;

        // Ações ao entrar em cada estado
        switch (estadoAtual)
        {
            case EstadoDoJogo.Erro:
                ExecutarErro();
                break;
            case EstadoDoJogo.Sucesso:
                ExecutarSucesso();
                break;
        }
    }

    // 3. Validação das energias
    private bool ExistemEnergiasAtivas()
    {
        foreach (Energia e in listaDeEnergias)
        {
            if (e.energiaAtiva) return true; // Encontrou um perigo
        }
        return false; // Tudo seguro
    }

    // 4. Integração com zona de risco
    // Este método deve ser chamado pelo script da "Zona de Risco" (Trigger)
    public void ProcessarEntradaNaZona()
    {
        if (ExistemEnergiasAtivas())
        {
            AlterarEstado(EstadoDoJogo.Erro);
        }
        else
        {
            AlterarEstado(EstadoDoJogo.Sucesso);
        }
    }

    private void ExecutarErro()
    {
        Debug.LogError("<color=red>[LOG ESTRUTURADO]</color> Sistema ainda energizado. Validação falhou!");
        if (somAlarme != null) somAlarme.Play();
    }

    private void ExecutarSucesso()
    {
        Debug.Log("<color=green>[LOG ESTRUTURADO]</color> Validação concluída com sucesso. Sistema seguro.");
    }
}