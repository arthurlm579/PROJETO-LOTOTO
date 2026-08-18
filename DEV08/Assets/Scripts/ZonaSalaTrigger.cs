using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct TarefaExtra
{
    public string descricao;
    public bool concluida;
}

[RequireComponent(typeof(Collider))]
public class ZonaSalaTrigger : MonoBehaviour
{
    // Guarda estaticamente qual é a zona em que o jogador está atualmente
    public static ZonaSalaTrigger ZonaAtual { get; private set; }

    [Header("Identificação da Sala/Zona")]
    public string nomeDaSala = "Setor de Caldeiras";

    [Header("Energias desta Sala")]
    [Tooltip("Arraste aqui todas as energias/painéis contidos dentro desta sala.")]
    public List<EnergiaBase> energiasDaSala = new List<EnergiaBase>();

    [Header("Tarefas Secundárias Locais")]
    public List<TarefaExtra> tarefasSecundarias = new List<TarefaExtra>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Define esta sala como a zona ativa atual
            ZonaAtual = this;
            AtualizarTarefasUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // REMOVIDO: Não limpamos mais a UI ao sair!
            // As missões permanecem na tela até o jogador entrar no Trigger de OUTRA sala.
        }
    }

    private void Update()
    {
        // Se esta sala for a zona ativa atual, atualiza o status das energias em tempo real na UI
        if (ZonaAtual == this)
        {
            AtualizarTarefasUI();
        }
    }

    public void AtualizarTarefasUI()
    {
        if (TaskManagerUI.Instance == null) return;

        int energiasLigadas = 0;
        int totalEnergias = energiasDaSala.Count;

        // Contagem de energias ativas na sala
        foreach (EnergiaBase energia in energiasDaSala)
        {
            if (energia != null && energia.energiaAtiva)
            {
                energiasLigadas++;
            }
        }

        // Formatação das tarefas secundárias
        List<string> listaTarefasTexto = new List<string>();
        foreach (var t in tarefasSecundarias)
        {
            string status = t.concluida ? "<color=#55FF55>[✓]" : "<color=#FF8800>[-]";
            listaTarefasTexto.Add($"{status} {t.descricao}</color>");
        }

        // Atualiza a interface da UI com os dados desta sala
        TaskManagerUI.Instance.AtualizarPainelTarefas(
            nomeDaSala,
            energiasLigadas,
            totalEnergias,
            listaTarefasTexto.ToArray()
        );
    }

    /// <summary>
    /// Função pública para concluir uma tarefa secundária da sala por código ou evento.
    /// </summary>
    public void ConcluirTarefaSecundaria(int indice)
    {
        if (indice >= 0 && indice < tarefasSecundarias.Count)
        {
            TarefaExtra t = tarefasSecundarias[indice];
            t.concluida = true;
            tarefasSecundarias[indice] = t;

            if (ZonaAtual == this)
            {
                AtualizarTarefasUI();
            }
        }
    }
}