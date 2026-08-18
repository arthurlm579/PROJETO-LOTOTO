using UnityEngine;

// Enumeração para identificar os tipos de tranca no jogo
public enum TipoTranca
{
    BloqueioEletrico,
    BloqueioValvulaHidraulica,
    BloqueioPneumatico,
    Nenhum
}

public class MenuSelecaoTrancaUI : MonoBehaviour
{
    public static MenuSelecaoTrancaUI Instance;

    [Header("UI do Menu")]
    public GameObject painelSelecao;

    // Guarda temporariamente qual objeto está tentando ser trancado
    private object objetoParaTrancar;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (painelSelecao != null)
            painelSelecao.SetActive(false);
    }

    // Abre o menu e salva quem chamou a abertura
    public void AbrirMenu(object objetoInteragivel)
    {
        objetoParaTrancar = objetoInteragivel;

        if (painelSelecao != null)
            painelSelecao.SetActive(true);

        // Libera o cursor para clicar nos botões do menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void FecharMenu()
    {
        if (painelSelecao != null)
            painelSelecao.SetActive(false);

        objetoParaTrancar = null;

        // Trava o cursor de volta para o modo Primeira Pessoa
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Funções chamadas pelos botões na UI do Menu:
    public void BotaoEscolherEletrico()
    {
        ConfirmarEscolha(TipoTranca.BloqueioEletrico);
    }

    public void BotaoEscolherValvula()
    {
        ConfirmarEscolha(TipoTranca.BloqueioValvulaHidraulica);
    }

    public void BotaoEscolherPneumatico()
    {
        ConfirmarEscolha(TipoTranca.BloqueioPneumatico);
    }

    private void ConfirmarEscolha(TipoTranca trancaEscolhida)
    {
        // 1. Salva a referência antes de limpar
        object objetoTemp = objetoParaTrancar;

        // 2. Fecha o menu primeiro para liberar a tela e o cursor
        FecharMenu();

        // 3. Agora chama a validação e exibe a mensagem de erro/sucesso!
        if (objetoTemp is ValveWheelInteractable valvula)
        {
            valvula.ValidarETrancar(trancaEscolhida);
        }
        else if (objetoTemp is WorldSpaceButtonInteractable botao)
        {
            botao.ValidarETrancar(trancaEscolhida);
        }
    }
}