using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum EstadoDoJogo { Iniciando, EmJogo, Erro, Sucesso, Pausado }

    [Header("Configurações de Fluxo")]
    [SerializeField] private EstadoDoJogo estadoAtual = EstadoDoJogo.Iniciando;

    [Header("Referências Globais")]
    [SerializeField] private AudioSource somAlarme;

    // Lista opcional apenas para você acompanhar no inspetor quais energias existem
    [SerializeField] private List<EnergiaBase> todasAsEnergias;

    void Start()
    {
        AlterarEstado(EstadoDoJogo.EmJogo);
    }

    public void AlterarEstado(EstadoDoJogo novoEstado)
    {
        // Bloqueio do sistema: Impede novas mudanças se o jogo já terminou em Erro ou Sucesso
        if (estadoAtual == EstadoDoJogo.Sucesso || estadoAtual == EstadoDoJogo.Erro)
        {
            Debug.LogWarning($"Movimento bloqueado: O jogo já foi finalizado como {estadoAtual}.");
            return;
        }

        Debug.Log($"<color=white>Transição de Estado:</color> {estadoAtual} -> <b>{novoEstado}</b>");
        estadoAtual = novoEstado;

        switch (estadoAtual)
        {
            case EstadoDoJogo.Erro:
                ExecutarErroGlobal();
                break;
            case EstadoDoJogo.Sucesso:
                ExecutarSucessoGlobal();
                break;
        }
    }

    // Integração com as zonas de risco individuais
    public void ProcessarEntradaNaZona(EnergiaBase energiaValidada)
    {
        // Se o jogo já acabou ou está pausado, ignora a validação da zona
        if (estadoAtual != EstadoDoJogo.EmJogo) return;

        if (energiaValidada.energiaAtiva)
        {
            // Log estruturado dizendo exatamente qual zona causou o acidente
            Debug.LogError($"<color=red>[FALHA CRÍTICA]</color> O jogador entrou na zona de risco com a fonte <b>{energiaValidada.nomeEnergia}</b> ({energiaValidada.tipoDefinido}) ainda ativa!");
            AlterarEstado(EstadoDoJogo.Erro);
        }
        else
        {
            // Log estruturado informando que esta zona específica está segura
            Debug.Log($"<color=green>[ZONA SEGURA]</color> Validação aprovada para <b>{energiaValidada.nomeEnergia}</b> ({energiaValidada.tipoDefinido}).");

            // Opcional: Verificar se todas as outras também já foram desligadas para dar a vitória final
            ChecarVitoriaCompleta();
        }
    }

    private void ChecarVitoriaCompleta()
    {
        foreach (EnergiaBase e in todasAsEnergias)
        {
            if (e.energiaAtiva) return; // Se ainda tem alguma ativa no mapa, o jogo continua
        }

        // Se passou por todas e nenhuma está ativa, o jogador isolou o mapa inteiro com sucesso
        AlterarEstado(EstadoDoJogo.Sucesso);
    }

    private void ExecutarErroGlobal()
    {
        Debug.LogError("<color=red><b>[LOG ESTRUTURADO]</b></color> Simulação encerrada devido a acidente de trabalho.");
        if (somAlarme != null) somAlarme.Play();
    }

    private void ExecutarSucessoGlobal()
    {
        Debug.Log("<color=green><b>[LOG ESTRUTURADO]</b></color> Fantástico! Todas as zonas foram totalmente isoladas de forma segura.");
    }
}