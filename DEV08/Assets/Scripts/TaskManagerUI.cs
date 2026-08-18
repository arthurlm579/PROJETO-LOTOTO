using UnityEngine;
using TMPro; // Use UnityEngine.UI se não usar TextMeshPro

public class TaskManagerUI : MonoBehaviour
{
    public static TaskManagerUI Instance;

    [Header("Componentes de UI")]
    [Tooltip("Arraste aqui o TextMeshProUGUI que exibirá a lista de tarefas no Canvas")]
    [SerializeField] private TMP_Text textoListaTarefas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Atualiza o painel de tarefas com o nome da sala, a contagem de energias e as tarefas secundárias.
    /// </summary>
    public void AtualizarPainelTarefas(string nomeSala, int energiasLigadas, int totalEnergias, string[] tarefasSecundarias)
    {
        if (textoListaTarefas == null) return;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // Cabeçalho da Zona
        sb.AppendLine($"<b><color=#FFCC00>=== {nomeSala.ToUpper()} ===</color></b>");
        sb.AppendLine();

        // Tarefa Principal (Isolamento LOTO)
        sb.AppendLine("<b><color=#FFFFFF>• Tarefa Principal:</color></b>");
        if (energiasLigadas > 0)
        {
            sb.AppendLine($"  <color=#FF5555>[-] Desligar e Bloquear Energias ({energiasLigadas}/{totalEnergias} ativas)</color>");
        }
        else
        {
            sb.AppendLine($"  <color=#55FF55>[✓] Todas as energias desta zona foram isoladas!</color>");
        }

        // Tarefas Secundárias da Sala (se houver)
        if (tarefasSecundarias != null && tarefasSecundarias.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine("<b><color=#FFFFFF>• Objetivos Locais:</color></b>");
            foreach (string tarefa in tarefasSecundarias)
            {
                sb.AppendLine($"  <color=#AAAAAA>- {tarefa}</color>");
            }
        }

        textoListaTarefas.text = sb.ToString();
    }

    /// <summary>
    /// Limpa o painel quando o jogador sai de qualquer sala registrada.
    /// </summary>
    public void LimparPainel()
    {
        if (textoListaTarefas != null)
        {
            textoListaTarefas.text = "<b><color=#888888>Entre em uma Zona de Trabalho para ver as Tasks.</color></b>";
        }
    }
}