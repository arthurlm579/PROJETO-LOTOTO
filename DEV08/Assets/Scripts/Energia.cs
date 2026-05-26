using UnityEngine;

public class EnergiaBase : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public string nomeEnergia;
    public string tipoDefinido; // Guardará o nome do tipo (Ex: "Elétrica")

    [SerializeField] protected bool _energiaAtiva = true;
    [SerializeField] protected bool _bloqueada = false;

    public bool energiaAtiva => _energiaAtiva;
    public bool bloqueada => _bloqueada;

    public virtual void Desligar()
    {
        if (!_bloqueada)
        {
            _energiaAtiva = false;
            Debug.Log($"<color=orange>{nomeEnergia}:</color> {tipoDefinido} desligada.");
        }
    }

    public virtual void Bloquear()
    {
        if (!_energiaAtiva)
        {
            _bloqueada = true;
            Debug.Log($"<color=blue>{nomeEnergia}:</color> Bloqueio {tipoDefinido} aplicado.");
        }
    }
}