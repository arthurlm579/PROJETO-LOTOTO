using UnityEngine;

public class ValveWheelInteractable : MonoBehaviour
{
    [Header("Conexão com Sistema de Energia")]
    public Transform objetoEnergiaTransform;

    [Header("Sistema de Proximidade")]
    public Transform playerTransform;
    public float distanciaInteracao = 2.5f;

    [Header("Referências Visuais")]
    public Transform wheelTransform;

    [Header("Sistema de Bloqueio Físico (LOTO)")]
    public GameObject capaBloqueioVisual;
    public bool estaTrancada = false;

    [Header("Validação de Tranca Correta")]
    [Tooltip("Escolha qual é a tranca correta para esta Válvula")]
    public TipoTranca trancaCorreta = TipoTranca.BloqueioValvulaHidraulica;

    [Header("Controle de Teclas")]
    public KeyCode rotateLeftKey = KeyCode.Q;
    public KeyCode rotateRightKey = KeyCode.E;
    public KeyCode teclaTrancar = KeyCode.F;

    [Header("Configurações de Rotação")]
    public float rotationSpeed = 95f;
    public Vector3 rotationAxis = Vector3.forward;

    [Header("Limites da Válvula")]
    public bool useRotationLimit = true;
    public float minRotation = 0f;
    public float maxRotation = 180f;
    public bool startsOpen = false;

    private float currentRotation;
    private Quaternion initialWheelRotation;
    private bool jaDesligouEnergia = false;
    private bool playerEstaPerto = false;

    void Start()
    {
        if (wheelTransform == null) wheelTransform = transform;

        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }

        initialWheelRotation = wheelTransform.localRotation;
        currentRotation = startsOpen ? maxRotation : minRotation;
        ApplyWheelRotation();

        AtualizarVisualBloqueio();
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanciaAtual = Vector3.Distance(transform.position, playerTransform.position);

        if (distanciaAtual <= distanciaInteracao)
        {
            playerEstaPerto = true;

            bool cartaoEquipado = (InventorySystem.Instance != null && InventorySystem.Instance.cartaoEquipado);

            if (cartaoEquipado)
            {
                string acaoTexto = estaTrancada ? "Remover capa de proteção" : "Escolher bloqueio LOTO";
                if (InteractionUI.Instance != null)
                    InteractionUI.Instance.Mostrar($"<color=green>[F]</color> {acaoTexto}");

                if (Input.GetKeyDown(teclaTrancar))
                {
                    if (estaTrancada)
                    {
                        // Se já está trancada, destranca direto
                        DefinirTrancamento(false);
                        InteractionUI.Instance?.MostrarPorTempo("<color=green>[LOTO]</color> Capa de proteção removida!", 3.0f);
                    }
                    else
                    {
                        // Se está aberta, abre o menu para escolher o tipo de tranca!
                        if (MenuSelecaoTrancaUI.Instance != null)
                        {
                            MenuSelecaoTrancaUI.Instance.AbrirMenu(this);
                        }
                    }
                }
            }
            else
            {
                if (estaTrancada)
                {
                    if (InteractionUI.Instance != null)
                        InteractionUI.Instance.Mostrar("<color=red>[BLOQUEADO]</color> Trancada! Equipe o Cartão no [TAB]");
                }
                else
                {
                    if (InteractionUI.Instance != null)
                        InteractionUI.Instance.Mostrar("Pressione 'Q' ou 'E' para girar | Equipe o Cartão para Trancar");
                }
            }

            if (!estaTrancada)
            {
                HandleValveRotation();
            }
        }
        else
        {
            if (playerEstaPerto)
            {
                playerEstaPerto = false;
                if (InteractionUI.Instance != null) InteractionUI.Instance.Esconder();
            }
        }
    }

    // Método que é chamado após escolher uma tranca na UI:
    public void ValidarETrancar(TipoTranca trancaEscolhida)
    {
        if (trancaEscolhida == trancaCorreta)
        {
            DefinirTrancamento(true);
            if (InteractionUI.Instance != null)
                InteractionUI.Instance.MostrarPorTempo("<color=green>[SUCESSO]</color> Bloqueio correto aplicado na Válvula!", 3.5f);

            Debug.Log("<color=green>[LOTO]</color> Tranca correta aplicada com sucesso!");
        }
        else
        {
            if (InteractionUI.Instance != null)
                InteractionUI.Instance.MostrarPorTempo("<color=red>[ERRO LOTO]</color> Tipo de tranca incorreto! Esta é uma energia hidráulica/fluida.", 4.0f);

            Debug.LogWarning("<color=red>[LOTO]</color> Jogador escolheu o dispositivo de bloqueio errado!");
        }
    }

    void HandleValveRotation()
    {
        float input = 0f;
        if (Input.GetKey(rotateLeftKey)) input -= 1f;
        if (Input.GetKey(rotateRightKey)) input += 1f;

        if (Mathf.Abs(input) < 0.1f)
        {
            ApplyWheelRotation();
            return;
        }

        currentRotation += input * rotationSpeed * Time.deltaTime;
        if (useRotationLimit) currentRotation = Mathf.Clamp(currentRotation, minRotation, maxRotation);

        ApplyWheelRotation();
        VerificarDesligamentoDeEnergia();
    }

    void ApplyWheelRotation()
    {
        if (wheelTransform == null) return;
        Quaternion spinRotation = Quaternion.AngleAxis(currentRotation, rotationAxis.normalized);
        wheelTransform.localRotation = initialWheelRotation * spinRotation;
    }

    private void VerificarDesligamentoDeEnergia()
    {
        if (jaDesligouEnergia || objetoEnergiaTransform == null) return;

        if ((startsOpen && currentRotation <= minRotation) || (!startsOpen && currentRotation >= maxRotation))
        {
            jaDesligouEnergia = true;
            EnergiaBase energia = objetoEnergiaTransform.GetComponent<EnergiaBase>();
            if (energia != null) energia.Desligar();
        }
    }

    public void DefinirTrancamento(bool trancar)
    {
        estaTrancada = trancar;
        AtualizarVisualBloqueio();
    }

    private void AtualizarVisualBloqueio()
    {
        if (capaBloqueioVisual != null) capaBloqueioVisual.SetActive(estaTrancada);
    }
}