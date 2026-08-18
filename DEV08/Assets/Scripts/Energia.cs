using UnityEngine;

public class EnergiaBase : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public string nomeEnergia;
    public string tipoDefinido;

    [SerializeField] protected bool _energiaAtiva = true;
    [SerializeField] protected bool _bloqueada = false;

    public bool energiaAtiva => _energiaAtiva;
    public bool bloqueada => _bloqueada;

    protected virtual void Start()
    {
        // Verifica se já existe um valor salvo para essa energia
        if (PlayerPrefs.HasKey($"{nomeEnergia}_Ativa"))
        {
            // Usa o SaveSystem para restaurar o estado da energia
            _energiaAtiva = SaveSystem.CarregarEstadoAtivo(nomeEnergia, _energiaAtiva);
            _bloqueada = SaveSystem.CarregarEstadoBloqueado(nomeEnergia, _bloqueada);
        }

        // Registra a energia no GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarEnergia(this);
        }
    }

    public virtual void Desligar()
    {
        if (!_bloqueada)
        {
            _energiaAtiva = false;
            Debug.Log($"<color=orange>{nomeEnergia}:</color> {tipoDefinido} desligada.");

            if (GameManager.Instance != null)
                GameManager.Instance.NotificarMundancaDeEstado();
        }
    }

    public virtual void Bloquear()
    {
        if (!_energiaAtiva)
        {
            _bloqueada = true;
            Debug.Log($"<color=blue>{nomeEnergia}:</color> Bloqueio {tipoDefinido} aplicado.");

            if (GameManager.Instance != null)
                GameManager.Instance.NotificarMundancaDeEstado();
        }
    }
}