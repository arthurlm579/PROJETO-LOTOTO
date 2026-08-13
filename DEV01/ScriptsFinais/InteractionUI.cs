using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance;

    [SerializeField] private TextMeshProUGUI promptText;

    void Awake()
    {
        // Padrão Singleton para conseguir chamar este texto de qualquer outro script facilmente
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Esconder();
    }

    // Mostra o texto na tela com a mensagem desejada
    public void Mostrar(string mensagem)
    {
        if (promptText != null)
        {
            promptText.text = mensagem;
            promptText.gameObject.SetActive(true);
        }
    }

    // Esconde o texto da tela
    public void Esconder()
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }
}