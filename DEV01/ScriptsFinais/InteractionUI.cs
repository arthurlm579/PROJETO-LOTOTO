using UnityEngine;
using TMPro;
using System.Collections;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance;

    [SerializeField] private TextMeshProUGUI promptText;

    private Coroutine rotinaEsconder;
    private bool mensagemBloqueadaPorTempo = false; // Impede que o Update apague o erro!

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        EsconderForcado();
    }

    // Mostra texto de mira normal (pode ser sobrescrito a qualquer momento)
    public void Mostrar(string mensagem)
    {
        // Se estiver exibindo uma mensagem temporária importante (ex: ERRO), ignora chamadas comuns
        if (mensagemBloqueadaPorTempo) return;

        if (promptText != null)
        {
            promptText.text = mensagem;
            promptText.gameObject.SetActive(true);
        }
    }

    // Mostra mensagens importantes de Sucesso/Erro que NÃO sobem com o movimento da câmera
    public void MostrarPorTempo(string mensagem, float tempoSegundos = 3.5f)
    {
        if (rotinaEsconder != null)
        {
            StopCoroutine(rotinaEsconder);
        }

        if (promptText != null)
        {
            promptText.text = mensagem;
            promptText.gameObject.SetActive(true);
        }

        mensagemBloqueadaPorTempo = true;
        rotinaEsconder = StartCoroutine(ContagemEsconder(tempoSegundos));
    }

    private IEnumerator ContagemEsconder(float tempo)
    {
        yield return new WaitForSeconds(tempo);
        mensagemBloqueadaPorTempo = false;
        EsconderForcado();
        rotinaEsconder = null;
    }

    // Esconde o texto, respeitando se houver mensagem bloqueada por tempo
    public void Esconder()
    {
        if (mensagemBloqueadaPorTempo) return;

        EsconderForcado();
    }

    // Força o fechamento imediato (usado internamente)
    private void EsconderForcado()
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }
}