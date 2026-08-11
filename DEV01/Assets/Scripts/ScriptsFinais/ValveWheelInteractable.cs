using UnityEngine;

public class ValveWheelInteractable : MonoBehaviour
{
    [Header("Conexão com Sistema de Energia")]
    [Tooltip("Arraste aqui o GameObject que possui o script de Energia")]
    public Transform objetoEnergiaTransform;

    [Header("Sistema de Proximidade")]
    [Tooltip("Arraste o Transform do Player aqui (ou o script achará automaticamente pela Tag 'Player')")]
    public Transform playerTransform;

    [Tooltip("Distância máxima (em metros) que o Player pode estar para conseguir girar a válvula")]
    public float distanciaInteracao = 2.5f;

    [Header("Referências Visuais")]
    public Transform wheelTransform;

    [Header("Controle de Teclas (Apenas Q e E)")]
    public KeyCode rotateLeftKey = KeyCode.Q;
    public KeyCode rotateRightKey = KeyCode.E;

    [Header("Configurações de Rotação")]
    public float rotationSpeed = 95f;

    [Tooltip("Eixo local da roda (ex: Y = 1 para horizontal, Z = 1 para vertical)")]
    public Vector3 rotationAxis = Vector3.forward;

    [Header("Limites da Válvula (Ângulo)")]
    public bool useRotationLimit = true;
    public float minRotation = 0f;
    public float maxRotation = 180f;

    [Header("Estado Inicial")]
    public bool startsOpen = false;

    private float currentRotation;
    private Quaternion initialWheelRotation;
    private bool jaDesligouEnergia = false;
    private bool playerEstaPerto = false;

    void Start()
    {
        if (wheelTransform == null) wheelTransform = transform;

        // Procura automaticamente o Player pela Tag caso não esteja atribuído no Inspector
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
        }

        initialWheelRotation = wheelTransform.localRotation;
        currentRotation = startsOpen ? maxRotation : minRotation;
        ApplyWheelRotation();
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Calcula a distância do Player
        float distanciaAtual = Vector3.Distance(transform.position, playerTransform.position);

        if (distanciaAtual <= distanciaInteracao)
        {
            playerEstaPerto = true;

            // Exibe a dica na tela enquanto o jogador estiver perto
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.Mostrar("Pressione 'Q' ou 'E' para girar a válvula");
            }

            HandleValveRotation();
        }
        else
        {
            // Esconde a dica quando o jogador se afasta
            if (playerEstaPerto)
            {
                playerEstaPerto = false;
                if (InteractionUI.Instance != null)
                {
                    InteractionUI.Instance.Esconder();
                }
            }
        }
    }

    void HandleValveRotation()
    {
        float input = 0f;

        // Aceita EXCLUSIVAMENTE as teclas Q e E
        if (Input.GetKey(rotateLeftKey)) input -= 1f;
        if (Input.GetKey(rotateRightKey)) input += 1f;

        if (Mathf.Abs(input) < 0.1f)
        {
            ApplyWheelRotation();
            return;
        }

        currentRotation += input * rotationSpeed * Time.deltaTime;

        if (useRotationLimit)
        {
            currentRotation = Mathf.Clamp(currentRotation, minRotation, maxRotation);
        }

        ApplyWheelRotation();
        VerificarDesligamentoDeEnergia();
    }

    void ApplyWheelRotation()
    {
        if (wheelTransform == null) return;

        Quaternion spinRotation = Quaternion.AngleAxis(
            currentRotation,
            rotationAxis.normalized
        );

        wheelTransform.localRotation = initialWheelRotation * spinRotation;
    }

    private void VerificarDesligamentoDeEnergia()
    {
        if (jaDesligouEnergia || objetoEnergiaTransform == null) return;

        if (startsOpen && currentRotation <= minRotation)
        {
            DesligarEnergia();
        }
        else if (!startsOpen && currentRotation >= maxRotation)
        {
            DesligarEnergia();
        }
    }

    private void DesligarEnergia()
    {
        jaDesligouEnergia = true;

        EnergiaBase energia = objetoEnergiaTransform.GetComponent<EnergiaBase>();

        if (energia != null)
        {
            energia.Desligar();
            Debug.Log($"<color=green>[VÁLVULA]</color> Energia {energia.nomeEnergia} foi desligada.");
        }
        else
        {
            Debug.LogError($"<color=red>[ERRO]</color> O objeto '{objetoEnergiaTransform.name}' não possui um script de energia!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, distanciaInteracao);
    }
}