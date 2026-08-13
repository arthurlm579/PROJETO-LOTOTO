using UnityEngine;
using System.Collections;

public class EnergiaHidraulica : EnergiaBase // Agora herda do molde correto
{
    [Header("Configurações Visuais")]
    [Tooltip("Arraste o MeshRenderer aqui se ele estiver em um objeto filho. Se deixar vazio, ele busca no próprio objeto.")]
    [SerializeField] private MeshRenderer objetoRender;

    [Header("Cores do Indicador")]
    public Color corAtivo = Color.blue;
    public Color corDesligado = Color.gray;

    [Header("Configurações de Tempo")]
    [Tooltip("Se marcado, aguarda o tempo definido antes de desligar visualmente")]
    public bool usarDelay = false;
    public float tempoDelay = 2.0f;

    void Awake()
    {
        // Se você não arrastou nada no Inspector, busca no próprio GameObject ou nos filhos
        if (objetoRender == null)
        {
            objetoRender = GetComponentInChildren<MeshRenderer>();
        }
    }

    void Start()
    {
        // Define o nome do tipo para os logs automáticos do GameManager
        tipoDefinido = "Energia Hidráulica";

        AtualizarVisual();
    }

    // Adaptando o seu método antigo para encaixar no comando padrão "Desligar"
    public override void Desligar()
    {
        // Se não estiver bloqueada e ainda estiver ativa, inicia o processo
        if (!_bloqueada && _energiaAtiva)
        {
            if (usarDelay)
            {
                StartCoroutine(RotinaDelay(tempoDelay));
            }
            else
            {
                // Desliga instantaneamente se a caixinha 'usarDelay' estiver desmarcada
                _energiaAtiva = false;
                AtualizarVisual();
                Debug.Log($"<color=orange>{nomeEnergia}:</color> {tipoDefinido} desligada instantaneamente.");
            }
        }
    }

    IEnumerator RotinaDelay(float tempo)
    {
        yield return new WaitForSeconds(tempo);

        _energiaAtiva = false; // Usa a variável interna da EnergiaBase
        AtualizarVisual();

        Debug.Log($"<color=orange>{nomeEnergia}:</color> {tipoDefinido} desligada após {tempo}s.");
    }

    // Sobrescreve o método Bloquear para atualizar o visual também
    public override void Bloquear()
    {
        if (!_energiaAtiva)
        {
            _bloqueada = true;
            AtualizarVisual();
            Debug.Log($"<color=blue>{nomeEnergia}:</color> Bloqueio {tipoDefinido} aplicado.");
        }
    }

    public void AtualizarVisual()
    {
        if (objetoRender != null)
        {
            Color corAlvo = _energiaAtiva ? corAtivo : corDesligado;

            // Altera a cor usando a propriedade nativa e previne bugs de shader URP/Standard
            objetoRender.material.color = corAlvo;

            if (objetoRender.material.HasProperty("_BaseColor"))
            {
                objetoRender.material.SetColor("_BaseColor", corAlvo);
            }
        }
        else
        {
            Debug.LogWarning($"<color=yellow>[AVISO]</color> NENHUM MeshRenderer foi encontrado em '{gameObject.name}' para trocar a cor!");
        }
    }
}