using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceButtonInteractable : MonoBehaviour
{
    [Header("Conexão com Sistema de Energia")]
    [Tooltip("Arraste aqui o GameObject da cena que possui o script de energia")]
    public Transform objetoEnergiaTransform;

    [Header("Configuração de Interação por Raycast")]
    [Tooltip("Tecla utilizada para pressionar o botão quando olhar para ele")]
    public KeyCode teclaInteracao = KeyCode.F;

    [Tooltip("Distância máxima que o player pode estar da tela para interagir")]
    public float distanciaMaxima = 3.0f;

    [Header("Sistema de Bloqueio (Cadeado)")]
    [Tooltip("Imagem UI do cadeado no canto do botão/painel")]
    public Image imagemCadeado;

    [Tooltip("Estado do bloqueio (true = Trancado/Vermelho, false = Liberado/Verde)")]
    public bool estaTrancada = true;

    public Color corLiberado = Color.green;
    public Color corTrancado = Color.red;

    [Header("Mensagem da Interface")]
    public string mensagemDica = "[F] Interagir com Painel";

    private Button botaoUI;
    private Camera cameraPrincipal;
    private bool olhandoParaOBotao = false;

    void Start()
    {
        botaoUI = GetComponent<Button>();
        cameraPrincipal = Camera.main;

        if (botaoUI != null)
        {
            botaoUI.onClick.AddListener(PressionarBotao);
        }

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
                if (!olhandoParaOBotao)
                {
                    olhandoParaOBotao = true;
                    AtualizarMensagemDica();
                }
                return;
            }
        }

        if (olhandoParaOBotao)
        {
            olhandoParaOBotao = false;
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.Esconder();
            }
        }
    }

    private void AtualizarMensagemDica()
    {
        if (InteractionUI.Instance == null) return;

        if (estaTrancada)
        {
            if (InventorySystem.Instance != null && InventorySystem.Instance.cartaoEquipado)
            {
                InteractionUI.Instance.Mostrar("<color=green>[F]</color> Destrancar cadeado (Cartão Equipado)");
            }
            else
            {
                InteractionUI.Instance.Mostrar("<color=red>[BLOQUEADO]</color> Trancado! Equipe o Cartão LOTO no [TAB]");
            }
        }
        else
        {
            InteractionUI.Instance.Mostrar(mensagemDica);
        }
    }

    public void PressionarBotao()
    {
        // Se estiver trancado
        if (estaTrancada)
        {
            // Tenta destrancar se o cartão estiver equipado
            if (InventorySystem.Instance != null && InventorySystem.Instance.cartaoEquipado)
            {
                DefinirTrancamento(false);
                AtualizarMensagemDica();

                if (InteractionUI.Instance != null)
                    InteractionUI.Instance.Mostrar("<color=green>[LOTO]</color> Cadeado destrancado com sucesso!");

                Debug.Log("<color=green>[LOTO]</color> Cadeado do painel foi aberto!");
            }
            else
            {
                Debug.LogWarning("<color=red>[LOTO]</color> O botão está trancado! Equipe o cartão no TAB.");
                if (InteractionUI.Instance != null)
                    InteractionUI.Instance.Mostrar("<color=red>[BLOQUEADO]</color> Trancado! Equipe o Cartão LOTO no [TAB]");
            }
            return;
        }

        // Se já estiver destrancado, desliga a energia
        if (objetoEnergiaTransform == null)
        {
            Debug.LogError($"<color=red>[PAINEL UI]</color> Nenhum objeto de energia foi associado ao botão em '{gameObject.name}'!");
            return;
        }

        EnergiaBase energia = objetoEnergiaTransform.GetComponent<EnergiaBase>();

        if (energia != null)
        {
            energia.Desligar();
            Debug.Log($"<color=green>[PAINEL UI]</color> Botão pressionado! Energia '{energia.nomeEnergia}' foi desligada.");
        }
        else
        {
            Debug.LogWarning($"<color=yellow>[AVISO]</color> O objeto '{objetoEnergiaTransform.name}' não possui um script derivado de EnergiaBase!");
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

    private void OnDisable()
    {
        if (olhandoParaOBotao && InteractionUI.Instance != null)
        {
            InteractionUI.Instance.Esconder();
        }
    }
}