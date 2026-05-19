using UnityEngine;

public class Energia : MonoBehaviour
{
    [Header("Status da Fonte de Energia")]
    [Tooltip("Nome identificador deste ponto de energia.")]
    public string nomeEnergia;

    [Tooltip("Indica se a energia está fluindo.")]
    [SerializeField] private bool _energiaAtiva = true;

    [Tooltip("Indica se o bloqueio físico (cadeado) foi aplicado.")]
    [SerializeField] private bool _bloqueada = false;

    // Propriedade pública para que outros scripts possam ler o estado, mas não alterar diretamente
    public bool energiaAtiva => _energiaAtiva;
    public bool bloqueada => _bloqueada;

    /// <summary>
    /// Desliga a energia se o ponto não estiver bloqueado.
    /// </summary>
    public void Desligar()
    {
        if (!_bloqueada)
        {
            _energiaAtiva = false;
            Debug.Log($"<color=orange>{nomeEnergia}:</color> Energia desligada com sucesso.");
        }
        else
        {
            Debug.LogWarning($"{nomeEnergia}: Impossível desligar/ligar enquanto o bloqueio estiver ativo!");
        }
    }

    /// <summary>
    /// Aplica o bloqueio de segurança (LOTO), mas apenas se a energia já estiver desligada.
    /// </summary>
    public void Bloquear()
    {
        if (!_energiaAtiva)
        {
            _bloqueada = true;
            Debug.Log($"<color=blue>{nomeEnergia}:</color> Bloqueio de segurança <b>(LOTOTO)</b> aplicado.");
        }
        else
        {
            Debug.LogError($"{nomeEnergia}: ERRO DE SEGURANÇA! Desligue a energia antes de aplicar o bloqueio.");
        }
    }
}