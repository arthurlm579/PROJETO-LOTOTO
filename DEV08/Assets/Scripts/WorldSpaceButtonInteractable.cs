using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necessário para TextMeshPro e TextMeshProUGUI

public enum TipoEnergiaPainel
{
    Eletrica,
    Pneumatica,
    Hidrica,
    Termica
}

public class WorldSpaceButtonInteractable : MonoBehaviour
{
    [Header("Conexão com Sistema de Energia")]
    public Transform objetoEnergiaTransform;

    [Header("Tipo de Energia do Painel")]
    [Tooltip("Escolha qual tipo de energia este painel controla.")]
    public TipoEnergiaPainel tipoEnergia = TipoEnergiaPainel.Eletrica;

    [Header("Identificação Visual do Painel (Arraste seu texto aqui)")]
    [Tooltip("Arraste se for um TextMeshPro (3D ou UI)")]
    [SerializeField] private TMP_Text textoTextMeshPro;

    [Tooltip("Arraste se for um TextMesh 3D tradicional")]
    [SerializeField] private TextMesh textoPlaca3D;

    [Tooltip("Arraste se for um Text de UI tradicional")]
    [SerializeField] private Text textoPlacaUI;

    private string nomeDoTipoEnergia = "Energia";

    [Header("Configuração de Interação por Raycast")]
    public KeyCode teclaInteracao = KeyCode.F;
    public float distanciaMaxima = 3.0f;

    [Header("Sistema de Bloqueio (Cadeado)")]
    public Image imagemCadeado;
    public bool estaTrancada = false;

    [Header("Validação de Tranca Correta")]
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

        // Se nada foi arrastado no Inspector, tenta encontrar automaticamente nos filhos
        if (textoTextMeshPro == null) textoTextMeshPro = GetComponentInChildren<TMP_Text>();
        if (textoPlaca3D == null) textoPlaca3D = GetComponentInChildren<TextMesh>();
        if (textoPlacaUI == null) textoPlacaUI = GetComponentInChildren<Text>();

        AtualizarNomeETextoEnergia();
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

    private void AtualizarNomeETextoEnergia()
    {
        // Define o texto correspondente ao Enum escolhido no Inspector
        switch (tipoEnergia)
        {
            case TipoEnergiaPainel.Eletrica:
                nomeDoTipoEnergia = "ENERGIA ELÉTRICA";
                break;
            case TipoEnergiaPainel.Pneumatica:
                nomeDoTipoEnergia = "ENERGIA PNEUMÁTICA";
                break;
            case TipoEnergiaPainel.Hidrica:
                nomeDoTipoEnergia = "ENERGIA HÍDRICA";
                break;
            case TipoEnergiaPainel.Termica:
                nomeDoTipoEnergia = "ENERGIA TÉRMICA";
                break;
        }

        // Escreve no campo que estiver preenchido/encontrado
        if (textoTextMeshPro != null)
        {
            textoTextMeshPro.text = nomeDoTipoEnergia;
        }
        if (textoPlaca3D != null)
        {
            textoPlaca3D.text = nomeDoTipoEnergia;
        }
        if (textoPlacaUI != null)
        {
            textoPlacaUI.text = nomeDoTipoEnergia;
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
            InteractionUI.Instance.Mostrar($"<color=yellow>[{nomeDoTipoEnergia}]</color>\nPressione <color=green>[F]</color> para {acaoTexto}");
        }
        else
        {
            if (estaTrancada)
            {
                InteractionUI.Instance.Mostrar($"<color=yellow>[{nomeDoTipoEnergia}]</color>\n<color=red>[BLOQUEADO]</color> Equipe o Cartão no [TAB]");
            }
            else
            {
                InteractionUI.Instance.Mostrar($"<color=yellow>[{nomeDoTipoEnergia}]</color>\n[F] Pressionar Botão");
            }
        }
    }

    public void PressionarBotao()
    {
        bool cartaoEquipado = (InventorySystem.Instance != null && InventorySystem.Instance.cartaoEquipado);

        if (cartaoEquipado)
        {
            if (estaTrancada)
            {
                DefinirTrancamento(false);
                if (InteractionUI.Instance != null)
                    InteractionUI.Instance.MostrarPorTempo("<color=green>[LOTO]</color> Cadeado removido!", 3.0f);
            }
            else
            {
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

        if (objetoEnergiaTransform != null)
        {
            EnergiaBase energia = objetoEnergiaTransform.GetComponent<EnergiaBase>();
            if (energia != null) energia.Desligar();
        }
    }

    public void ValidarETrancar(TipoTranca trancaEscolhida)
    {
        if (trancaEscolhida == trancaCorreta)
        {
            DefinirTrancamento(true);
            if (InteractionUI.Instance != null)
                InteractionUI.Instance.MostrarPorTempo("<color=green>[SUCESSO]</color> Bloqueio aplicado com sucesso!", 3.5f);
        }
        else
        {
            if (InteractionUI.Instance != null)
                InteractionUI.Instance.MostrarPorTempo($"<color=red>[ERRO LOTO]</color> Tranca incorreta para {nomeDoTipoEnergia}!", 4.0f);
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