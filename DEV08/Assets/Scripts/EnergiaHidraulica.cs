using UnityEngine;
using System.Collections;

public class EnergiaHidraulica : EnergiaBase
{
    [Header("Configurações Visuais")]
    [Tooltip("Arraste o MeshRenderer aqui se ele estiver em um objeto filho. Se deixar vazio, ele busca no próprio objeto.")]
    [SerializeField] private MeshRenderer objetoRender;

    [Header("Configurações de Áudio")]
    [Tooltip("Arraste o AudioSource com o som do fluxo de fluido/pressão hidráulica em loop")]
    [SerializeField] private AudioSource somFluxoHidraulico;

    [Header("Cores do Indicador")]
    public Color corAtivo = Color.blue;
    public Color corDesligado = Color.gray;

    [Header("Configurações de Tempo")]
    public bool usarDelay = false;
    public float tempoDelay = 2.0f;

    void Awake()
    {
        if (objetoRender == null)
        {
            objetoRender = GetComponentInChildren<MeshRenderer>();
        }
    }

   public  void Start ()
    {
        tipoDefinido = "Energia Hidráulica";

        AtualizarVisualEAudio();
    }

    public override void Desligar()
    {
        if (!_bloqueada && _energiaAtiva)
        {
            if (usarDelay)
            {
                StartCoroutine(RotinaDelay(tempoDelay));
            }
            else
            {
                _energiaAtiva = false;
                AtualizarVisualEAudio();
                Debug.Log($"<color=orange>{nomeEnergia}:</color> {tipoDefinido} desligada instantaneamente.");
            }
        }
    }

    IEnumerator RotinaDelay(float tempo)
    {
        yield return new WaitForSeconds(tempo);

        _energiaAtiva = false;
        AtualizarVisualEAudio();

        Debug.Log($"<color=orange>{nomeEnergia}:</color> {tipoDefinido} desligada após {tempo}s.");
    }

    public override void Bloquear()
    {
        if (!_energiaAtiva)
        {
            _bloqueada = true;
            AtualizarVisualEAudio();
            Debug.Log($"<color=blue>{nomeEnergia}:</color> Bloqueio {tipoDefinido} aplicado.");
        }
    }

    public void AtualizarVisualEAudio()
    {
        // Atualização Visual
        if (objetoRender != null)
        {
            Color corAlvo = _energiaAtiva ? corAtivo : corDesligado;

            objetoRender.material.color = corAlvo;

            if (objetoRender.material.HasProperty("_BaseColor"))
            {
                objetoRender.material.SetColor("_BaseColor", corAlvo);
            }
        }

        // Atualização de Áudio (Novo)
        if (somFluxoHidraulico != null)
        {
            if (_energiaAtiva)
            {
                if (!somFluxoHidraulico.isPlaying) somFluxoHidraulico.Play();
            }
            else
            {
                if (somFluxoHidraulico.isPlaying) somFluxoHidraulico.Stop();
            }
        }
    }
}