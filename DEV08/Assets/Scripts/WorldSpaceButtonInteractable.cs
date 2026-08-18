using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceButtonInteractable : MonoBehaviour
{
    [Header("Conexão com Sistema de Energia")]
    public Transform objetoEnergiaTransform;

    [Header("Configuração de Interação por Raycast")]
    public KeyCode teclaInteracao = KeyCode.F;
    public float distanciaMaxima = 3.0f;

    [Header("Sistema de Bloqueio (Cadeado)")]
    public Image imagemCadeado;
    public bool estaTrancada = false;

    [Header("Validação de Tranca Correta")]
    [Tooltip("Escolha qual é a tranca correta para este Painel Elétrico")]
    public TipoTranca trancaCorreta = TipoTranca.BloqueioEletrico;

    public Color corLiberado = Color.green;
    public Color corTrancado = Color.red;

    private Button botaoUI;
    private Camera cameraPrincipal;
    private bool olhandoParaOBotao = false;

    void Start()
    {
        botaoUI = GetComponent<Button>();
        cameraPrincipal = Camera.main;

        if (botaoUI != null) botaoUI.onClick.AddListener(PressionarBotao);

        AtualizarStatusCadeado();
    }

    void Update()
    {
        ChecarVisaoPlayer();

        if (olhandoParaOBotao && Input.GetKeyDown(teclaInteracao))
        {
            PressionarBotao();
        }
    }

    private void ChecarVisaoPlayer()
    {
        if (cameraPrincipal == null) return;

        Ray ray = new Ray(cameraPrincipal.transform.position, cameraPrincipal.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaMaxima))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                if (!olhandoParaOBotao) olhandoParaOBotao = true;
                AtualizarMensagemDica();
                return;
            }
        }

        if (olhandoParaOBotao)
        {
            olhandoParaOBotao = false;
            if (InteractionUI.Instance != null) InteractionUI.Instance.Esconder();
        }
    }

    private void AtualizarMensagemDica()
    {
        if (InteractionUI.Instance == null) return;

        bool cartaoEquipado = (InventorySystem.Instance != null && InventorySystem.Instance.cartaoEquipado);

        if (cartaoEquipado)
        {
            string acaoTexto = estaTrancada ? "Remover Cadeado LOTO" : "Escolher Cadeado LOTO";
            InteractionUI.Instance.Mostrar($"<color=green>[F]</color> {acaoTexto}");
        }
        else
        {
            if (estaTrancada)
            {
                InteractionUI.Instance.Mostrar("<color=red>[BLOQUEADO]</color> Trancado! Equipe o Cartão no [TAB]");
            }
            else
            {
                InteractionUI.Instance.Mostrar("[F] Pressionar Botão do Painel");
            }
        }
    }

    public void PressionarBotao()
    {
        bool cartaoEquipado = (InventorySystem.Instance != null && InventorySystem.Instance.cartaoEquipado);

        // Se está com o cartão equipado
        if (cartaoEquipado)
        {
            if (estaTrancada)
            {
                // Se já está trancado, destranca direto
                DefinirTrancamento(false);
                if (InteractionUI.Instance != null)
                    InteractionUI.Instance.MostrarPorTempo("<color=green>[LOTO]</color> Cadeado removido!", 3.0f);
            }
            else
            {
                // Se está liberado, abre o menu para escolher a tranca!
                if (MenuSelecaoTrancaUI.Instance != null)
                {
                    MenuSelecaoTrancaUI.Instance.AbrirMenu(this);
                }
            }
            return;
        }

        if (estaTrancada)
        {
            Debug.LogWarning("<color=red>[LOTO]</color> O botão está trancado!");
            return;
        }

        // Se destrancado e sem o cartão selecionado, aciona a energia
        if (objetoEnergiaTransform != null)
        {
            EnergiaBase energia = objetoEnergiaTransform.GetComponent<EnergiaBase>();
            if (energia != null) energia.Desligar();
        }
    }

    // Chamado pelo Menu de Seleção após a escolha do botão:
    public void ValidarETrancar(TipoTranca trancaEscolhida)
    {
        if (trancaEscolhida == trancaCorreta)
        {
            DefinirTrancamento(true);
            if (InteractionUI.Instance != null)
                InteractionUI.Instance.MostrarPorTempo("<color=green>[SUCESSO]</color> Bloqueio Elétrico LOTO aplicado com sucesso!", 3.5f);

            Debug.Log("<color=green>[LOTO]</color> Tranca elétrica correta!");
        }
        else
        {
            if (InteractionUI.Instance != null)
                InteractionUI.Instance.MostrarPorTempo("<color=red>[ERRO LOTO]</color> Tipo de tranca incorreto! Painéis elétricos exigem garra/cadeado elétrico.", 4.0f);

            Debug.LogWarning("<color=red>[LOTO]</color> Jogador errou a tranca do painel!");
        }
    }

    public void DefinirTrancamento(bool trancar)
    {
        estaTrancada = trancar;
        AtualizarStatusCadeado();
    }

    private void AtualizarStatusCadeado()
    {
        if (imagemCadeado != null)
        {
            imagemCadeado.color = estaTrancada ? corTrancado : corLiberado;
        }
    }
}