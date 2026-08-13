using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum EstadoDoJogo { Iniciando, EmJogo, Erro, Sucesso, Pausado }

    [Header("Configurações de Fluxo")]
    [SerializeField] private EstadoDoJogo estadoAtual = EstadoDoJogo.Iniciando;

    [Header("Referências Globais")]
    [SerializeField] private AudioSource somAlarme;

    [Tooltip("Arraste as suas 4 energias específicas para esta lista no Inspector.")]
    [SerializeField] private List<EnergiaBase> todasAsEnergias = new List<EnergiaBase>();

    void Start()
    {
        AlterarEstado(EstadoDoJogo.EmJogo);
    }

    // =========================================================================
    // FUNÇÕES PARA OS BOTÕES INDIVIDUAIS (INTERFACE / UI)
    // =========================================================================

    // Botão 1: Desliga apenas a Energia Elétrica
    public void ComandoDesligarEletrica()
    {
        DesligarEnergiaPorTipo<EnergiaEletrica>();
    }

    // Botão 2: Desliga apenas a Energia Pneumática
    public void ComandoDesligarPneumatica()
    {
       // DesligarEnergiaPorTipo<EnergiaPneumatica>();
    }

    // Botão 3: Desliga apenas a Energia Hidráulica
    public void ComandoDesligarHidraulica()
    {
        DesligarEnergiaPorTipo<EnergiaHidraulica>();
    }

    // Botão 4: Desliga apenas a Energia Térmica / Química
    public void ComandoDesligarTermicaQuimica()
    {
        DesligarEnergiaPorTipo<EnergiaTermicaQuimica>();
    }


    // MÉTODO AUXILIAR: Procura na lista o script do tipo solicitado e o desliga
    private void DesligarEnergiaPorTipo<T>() where T : EnergiaBase
    {
        if (estadoAtual != EstadoDoJogo.EmJogo) return;

        foreach (EnergiaBase e in todasAsEnergias)
        {
            // Verifica se o script atual da lista é do tipo (T) que passamos na função
            if (e != null && e is T)
            {
                e.Desligar();
                Debug.Log($"<color=yellow>Comando Individual:</color> Solicitado desligamento de {e.tipoDefinido}.");
                return; // Encontrou e desligou, pode parar o loop
            }
        }

        Debug.LogWarning($"Aviso: Nenhuma energia do tipo {typeof(T).Name} foi encontrada na lista do GameManager.");
    }

    // =========================================================================
    // CONTROLE DE FLUXO CENTRALIZADO
    // =========================================================================
    public void AlterarEstado(EstadoDoJogo novoEstado)
    {
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

    // =========================================================================
    // INTEGRAÇÃO COM AS ZONAS DE RISCO INDIVIDUAIS
    // =========================================================================
    public void ProcessarEntradaNaZona(EnergiaBase energiaValidada)
    {
        if (estadoAtual != EstadoDoJogo.EmJogo) return;

        if (energiaValidada != null && energiaValidada.energiaAtiva)
        {
            Debug.LogError($"<color=red>[FALHA CRÍTICA]</color> O jogador entrou na zona de risco com a fonte <b>{energiaValidada.nomeEnergia}</b> ({energiaValidada.tipoDefinido}) ainda ativa!");
            AlterarEstado(EstadoDoJogo.Erro);
        }
        else if (energiaValidada != null)
        {
            Debug.Log($"<color=green>[ZONA SEGURA]</color> Validação aprovada para <b>{energiaValidada.nomeEnergia}</b> ({energiaValidada.tipoDefinido}).");
            ChecarVitoriaCompleta();
        }
    }

    private void ChecarVitoriaCompleta()
    {
        foreach (EnergiaBase e in todasAsEnergias)
        {
            if (e != null && e.energiaAtiva) return;
        }

        AlterarEstado(EstadoDoJogo.Sucesso);
    }

    private void ExecutarErroGlobal()
    {
        Debug.LogError("<color=red><b>[LOG ESTRUTURADO]</b></color> Simulação encerrada devido a acidente de trabalho. Zona de risco violada!");
        if (somAlarme != null && !somAlarme.isPlaying)
        {
            somAlarme.Play();
        }
    }

    private void ExecutarSucessoGlobal()
    {
        Debug.Log("<color=green><b>[LOG ESTRUTURADO]</b></color> Fantástico! Todas as zonas foram totalmente isoladas de forma segura. Energia zerada.");
        if (somAlarme != null && somAlarme.isPlaying)
        {
            somAlarme.Stop();
        }
    }
}