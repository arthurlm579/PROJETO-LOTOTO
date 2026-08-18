using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    [Header("UI do Inventário")]
    public GameObject painelInventario;

    [Header("Estado dos Itens")]
    public bool temCartaoDesbloqueio = false;
    public bool cartaoEquipado = false;

    private bool inventarioAberto = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (painelInventario != null)
            painelInventario.SetActive(false);
    }

    void Update()
    {
        // Tecla TAB abre e fecha o inventário
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            AlternarInventario();
        }
    }

    public void AlternarInventario()
    {
        inventarioAberto = !inventarioAberto;

        if (painelInventario != null)
            painelInventario.SetActive(inventarioAberto);

        // Destrava ou trava o mouse para conseguir clicar nos itens do inventário
        Cursor.lockState = inventarioAberto ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = inventarioAberto;
    }

    // Função chamada ao clicar no botão do Cartão dentro do Inventário
    public void EquiparDesequiparCartao()
    {
        if (!temCartaoDesbloqueio)
        {
            Debug.LogWarning("[INVENTÁRIO] Você ainda não coletou o cartão!");
            if (InteractionUI.Instance != null)
            {
                InteractionUI.Instance.MostrarPorTempo("<color=red>[INVENTÁRIO]</color> Você não possui o Cartão LOTO!", 2.0f);
            }
            return;
        }

        cartaoEquipado = !cartaoEquipado;

        string statusText = cartaoEquipado ? "<color=green>Cartão LOTO Equipado!</color>" : "<color=yellow>Cartão LOTO Desequipado!</color>";

        // Mostra na tela por 2.5 segundos e depois some sozinho!
        if (InteractionUI.Instance != null)
        {
            InteractionUI.Instance.MostrarPorTempo(statusText, 2.5f);
        }

        Debug.Log($"[INVENTÁRIO] Cartão LOTO: {cartaoEquipado}");
    }
    // Função para pegar o cartão no cenário (se quiser colocar um cartão numa mesa para pegar)
    public void ColetarCartao()
    {
        temCartaoDesbloqueio = true;
        Debug.Log("<color=green>[INVENTÁRIO]</color> Você adquiriu o Cartão de Desbloqueio LOTO!");
    }
}