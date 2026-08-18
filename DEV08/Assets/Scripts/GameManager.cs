using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum EstadoDoJogo { Iniciando, EmJogo, Erro, Sucesso, Pausado }

    [Header("Configurações de Fluxo")]
    [SerializeField] private EstadoDoJogo estadoAtual = EstadoDoJogo.Iniciando;

    [Header("Interface de Configurações / Pause")]
    [Tooltip("Arraste aqui o Painel de Configurações/Pause do seu Canvas")]
    [SerializeField] private GameObject painelConfiguracoes;

    [Header("Referências Globais")]
    [SerializeField] private AudioSource somAlarme;

    [Header("Monitoramento de Energias (Preenchido Automático)")]
    [SerializeField] private List<EnergiaBase> todasAsEnergias = new List<EnergiaBase>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (painelConfiguracoes != null)
            painelConfiguracoes.SetActive(false);

        AlterarEstado(EstadoDoJogo.EmJogo);
    }

    private void Update()
    {
        // Detecta o clique do ESC para pausar ou despausar o jogo
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AlternarPause();
        }
    }

    // =========================================================================
    // CONTROLE DE PAUSE E PAINEL DE CONFIGURAÇÕES
    // =========================================================================
    public void AlternarPause()
    {
        if (estadoAtual == EstadoDoJogo.Sucesso || estadoAtual == EstadoDoJogo.Erro) return;

        if (estadoAtual == EstadoDoJogo.Pausado)
        {
            // Despausa o jogo
            Time.timeScale = 1f;
            estadoAtual = EstadoDoJogo.EmJogo;

            if (painelConfiguracoes != null)
                painelConfiguracoes.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Pausa o jogo
            Time.timeScale = 0f;
            estadoAtual = EstadoDoJogo.Pausado;

            if (painelConfiguracoes != null)
                painelConfiguracoes.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // =========================================================================
    // SALVAMENTO E TROCA DE CENA
    // =========================================================================
    public void SalvarProgressoCena()
    {
        foreach (EnergiaBase e in todasAsEnergias)
        {
            if (e != null)
            {
                // Usa o SaveSystem para gravar o estado de cada energia
                SaveSystem.SalvarEstadoEnergia(e.nomeEnergia, e.energiaAtiva, e.bloqueada);
            }
        }
        Debug.Log("<color=green>[SAVE]</color> Progresso da cena salvo com sucesso!");
    }

    public void MudarDeCena(string nomeNovaCena)
    {
        // Salva as energias antes de carregar a próxima cena
        SalvarProgressoCena();

        // Garante que a velocidade do tempo volte ao normal antes de trocar
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeNovaCena);
    }

    // =========================================================================
    // REGISTRO DE ENERGIAS NO SISTEMA
    // =========================================================================
    public void RegistrarEnergia(EnergiaBase energia)
    {
        if (energia != null && !todasAsEnergias.Contains(energia))
        {
            todasAsEnergias.Add(energia);
        }
    }

    // =========================================================================
    // MÁQUINA DE ESTADOS DO JOGO
    // =========================================================================
    public void AlterarEstado(EstadoDoJogo novoEstado)
    {
        if (estadoAtual == EstadoDoJogo.Sucesso || estadoAtual == EstadoDoJogo.Erro) return;

        estadoAtual = novoEstado;

        switch (estadoAtual)
        {
            case EstadoDoJogo.Erro:
                ExecutarErroGlobal();
                break;
            case EstadoDoJogo.Sucesso:
                ExecutarSucessoGlobal();
                break;
        }
    }

    public void ProcessarEntradaNaZona(EnergiaBase energiaValidada)
    {
        if (estadoAtual != EstadoDoJogo.EmJogo) return;

        if (energiaValidada != null && energiaValidada.energiaAtiva)
        {
            Debug.LogError($"<color=red>[FALHA CRÍTICA]</color> Zona violada com {energiaValidada.nomeEnergia} ativa!");
            AlterarEstado(EstadoDoJogo.Erro);
        }
        else if (energiaValidada != null)
        {
            Debug.Log($"<color=green>[ZONA SEGURA]</color> Validação feita para {energiaValidada.nomeEnergia}.");
            ChecarVitoriaCompleta();
        }
    }

    public void NotificarMundancaDeEstado()
    {
        ChecarVitoriaCompleta();
    }

    private void ChecarVitoriaCompleta()
    {
        if (todasAsEnergias.Count == 0) return;

        foreach (EnergiaBase e in todasAsEnergias)
        {
            if (e != null && (e.energiaAtiva || !e.bloqueada))
            {
                return;
            }
        }

        AlterarEstado(EstadoDoJogo.Sucesso);
    }

    private void ExecutarErroGlobal()
    {
        if (somAlarme != null && !somAlarme.isPlaying)
        {
            somAlarme.Play();
        }

        if (InteractionUI.Instance != null)
        {
            InteractionUI.Instance.MostrarPorTempo("<color=red><b>[ACIDENTE DE TRABALHO]</b> ZONA INSEGURA VIOLADA!</color>", 10.0f);
        }
    }

    private void ExecutarSucessoGlobal()
    {
        if (somAlarme != null && somAlarme.isPlaying)
        {
            somAlarme.Stop();
        }

        if (InteractionUI.Instance != null)
        {
            InteractionUI.Instance.MostrarPorTempo("<color=green><b>[PROCEDIMENTO LOTO CONCLUÍDO]</b> Todas as energias foram isoladas e trancadas!</color>", 10.0f);
        }
    }

    public EstadoDoJogo ObterEstadoAtual() => estadoAtual;
}