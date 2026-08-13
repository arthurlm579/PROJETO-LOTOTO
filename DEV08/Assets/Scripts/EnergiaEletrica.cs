using UnityEngine;
using UnityEngine.InputSystem; // Mantém o suporte ao New Input System

public class EnergiaEletrica : EnergiaBase // Agora herda do molde correto
{
    [Header("Configurações do Disjuntor")]
    [SerializeField] private Light luzDoBloco;
    [SerializeField] private Color corLigado = Color.yellow;
    [SerializeField] private Color corDesligado = Color.red;

    private Renderer meuRenderer;

    void Start()
    {
        // Define o nome do tipo para os logs automáticos do GameManager
        tipoDefinido = "Energia Elétrica";

        meuRenderer = GetComponent<Renderer>();
        AtualizarSistemaVisual();
    }

    void Update()
    {
        // Se o sistema já estiver bloqueado, ignora novos cliques no disjuntor
        if (_bloqueada) return;

        // Mantém a sua lógica original de clique usando o New Input System
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray raio = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(raio, out hit))
            {
                if (hit.transform == this.transform)
                {
                    // Se clicar e estiver ativa, desliga. Se já estiver desligada, aplica o bloqueio.
                    if (_energiaAtiva)
                    {
                        Desligar();
                    }
                    else
                    {
                        Bloquear();
                    }
                }
            }
        }
    }

    // Sobrescreve o método padrão de desligamento da EnergiaBase
    public override void Desligar()
    {
        if (_bloqueada) return;

        if (_energiaAtiva)
        {
            _energiaAtiva = false; // Atualiza o estado lógico herdado da base
            AtualizarSistemaVisual();
            Debug.Log($"<color=orange>{nomeEnergia}:</color> Disjuntor desligado.");
        }
    }

    // Sobrescreve o método padrão de bloqueio da EnergiaBase
    public override void Bloquear()
    {
        if (!_energiaAtiva)
        {
            _bloqueada = true;
            AtualizarSistemaVisual();
            Debug.Log($"<color=blue>{nomeEnergia}:</color> Disjuntor bloqueado com cadeado de segurança.");
        }
    }

    // Mantém a sua excelente lógica de Feedback Visual Sincronizado
    private void UpdateVisuals(Color corParaAplicar)
    {
        if (meuRenderer != null)
        {
            meuRenderer.material.color = corParaAplicar;
        }

        if (luzDoBloco != null)
        {
            luzDoBloco.enabled = _energiaAtiva;
            luzDoBloco.color = corParaAplicar;
        }
    }

    private void AtualizarSistemaVisual()
    {
        Color corParaAplicar = _energiaAtiva ? corLigado : corDesligado;
        UpdateVisuals(corParaAplicar);
    }
}