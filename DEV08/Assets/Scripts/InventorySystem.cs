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
            return;
        }

        cartaoEquipado = !cartaoEquipado;

        string status = cartaoEquipado ? "EQUIPADO" : "DESEQUIPADO";
        Debug.Log($"<color=yellow>[INVENTÁRIO]</color> Cartão LOTO {status}!");

        if (InteractionUI.Instance != null)
        {
            InteractionUI.Instance.Mostrar($"Cartão LOTO: {status}");
        }
    }

    // Função para pegar o cartão no cenário (se quiser colocar um cartão numa mesa para pegar)
    public void ColetarCartao()
    {
        temCartaoDesbloqueio = true;
        Debug.Log("<color=green>[INVENTÁRIO]</color> Você adquiriu o Cartão de Desbloqueio LOTO!");
    }
}