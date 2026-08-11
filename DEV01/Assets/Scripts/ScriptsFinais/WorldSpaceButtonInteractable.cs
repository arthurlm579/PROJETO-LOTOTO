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

    [Header("Mensagem da Interface")]
    public string mensagemDica = "[F] Interagir com Painel";

    private Button botaoUI;
    private Camera cameraPrincipal;
    private bool olhandoParaOBotao = false;

    void Start()
    {
        botaoUI = GetComponent<Button>();
        cameraPrincipal = Camera.main;

        // Inscreve o evento de clique nativo da UI do Canvas
        if (botaoUI != null)
        {
            botaoUI.onClick.AddListener(PressionarBotao);
        }
    }

    void Update()
    {
        ChecarVisaoPlayer();

        // Aciona o botão ao apertar a tecla configurada (F)
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
            // Verifica se o Raycast atingiu este botão ou algum filho dele
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                if (!olhandoParaOBotao)
                {
                    olhandoParaOBotao = true;
                    if (InteractionUI.Instance != null)
                    {
                        InteractionUI.Instance.Mostrar(mensagemDica);
                    }
                }
                return;
            }
        }

        // Se o raio deixou de atingir o botão, esconde o texto de UI
        if (olhandoParaOBotao)
        {
            olhandoParaOBotao = false;
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.Esconder();
            }
        }
    }

    public void PressionarBotao()
    {
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

    private void OnDisable()
    {
        // Garante que o texto suma se o botão for desativado na cena
        if (olhandoParaOBotao && InteractionUI.Instance != null)
        {
            InteractionUI.Instance.Esconder();
        }
    }
}