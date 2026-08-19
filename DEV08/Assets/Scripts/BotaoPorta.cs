using UnityEngine;
using UnityEngine.UI; // Importante para manipular a Imagem do cadeado

public class BotaoPorta : MonoBehaviour
{
    [Header("Conexão com a Porta")]
    public GameObject portaObjeto;

    [Header("Controle de Acesso")]
    [Tooltip("Marque para permitir o uso do botão. Desmarque para trancá-lo (Cadeado).")]
    public bool podeSerAberta = true;

    [Header("Ícone do Cadeado")]
    [Tooltip("Arraste a Imagem (Image ou SpriteRenderer) do cadeado aqui")]
    public Image iconeCadeado;
    public Color corDestrancado = Color.green;
    public Color corTrancado = Color.red;

    [Header("Configuração do Raycast")]
    public float distanciaInteracao = 3.0f;
    public KeyCode teclaInteracao = KeyCode.F;

    private Camera cameraPrincipal;
    private PortaElevadica scriptPorta;
    private bool estaOlhandoParaOBotao = false;

    void Start()
    {
        cameraPrincipal = Camera.main;

        if (portaObjeto != null)
        {
            scriptPorta = portaObjeto.GetComponent<PortaElevadica>();
        }

        AtualizarCorCadeado();
    }

    void OnValidate()
    {
        // Atualiza a cor no Editor em tempo real ao marcar/desmarcar a caixinha
        AtualizarCorCadeado();
    }

    void Update()
    {
        if (cameraPrincipal == null || scriptPorta == null) return;

        Ray ray = new Ray(cameraPrincipal.transform.position, cameraPrincipal.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaInteracao) && (hit.transform == transform || hit.transform.IsChildOf(transform)))
        {
            if (!estaOlhandoParaOBotao)
            {
                estaOlhandoParaOBotao = true;

                if (InteractionUI.Instance != null)
                {
                    if (podeSerAberta)
                    {
                        InteractionUI.Instance.Mostrar("Pressione <color=green>[F]</color> para acionar a Porta");
                    }
                    else
                    {
                        InteractionUI.Instance.Mostrar("<color=red>[BLOQUEADO]</color> A porta está trancada!");
                    }
                }
            }

            if (Input.GetKeyDown(teclaInteracao) && podeSerAberta)
            {
                scriptPorta.AlternarPorta();
            }
        }
        else
        {
            if (estaOlhandoParaOBotao)
            {
                LimparUI();
            }
        }
    }

    public void AtualizarCorCadeado()
    {
        if (iconeCadeado != null)
        {
            iconeCadeado.color = podeSerAberta ? corDestrancado : corTrancado;
        }
    }

    private void OnDisable()
    {
        LimparUI();
    }

    private void LimparUI()
    {
        estaOlhandoParaOBotao = false;
        if (InteractionUI.Instance != null)
        {
            InteractionUI.Instance.Mostrar("");
        }
    }

    public void TravarBotao()
    {
        podeSerAberta = false;
        AtualizarCorCadeado();
    }

    public void DestravarBotao()
    {
        podeSerAberta = true;
        AtualizarCorCadeado();
    }
}