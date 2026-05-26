using UnityEngine;
using System.Collections;

public class EnergiaHidraulica : MonoBehaviour
{
    public string nomeEnergiaHidraulica;
    public bool energiaAtiva = true;
    public bool bloqueada = false;

    private MeshRenderer objetoRender;

    void Awake()
    {
        objetoRender = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        AtualizarVisual();
    }

    // Passo 2: Recebe a ordem do GameManager e espera o tempo
    public void IniciarDesligamento(float tempo)
    {
        if (!bloqueada && energiaAtiva)
        {
            StartCoroutine(RotinaDelay(tempo));
        }
    }

    IEnumerator RotinaDelay(float tempo)
    {
        yield return new WaitForSeconds(tempo);

        // Passo 1: Muda a cor para cinza após o tempo
        energiaAtiva = false;
        AtualizarVisual();
    }

    public void AtualizarVisual()
    {
        if (objetoRender != null)
        {
            // Azul se ativo, Cinza se desligado
            objetoRender.material.color = energiaAtiva ? Color.blue : Color.gray;
        }
    }

    // Validação segura para o Inspector do Unity
    void OnValidate()
    {
        // O TryGetComponent evita erros pesados no editor caso o componente mestre mude
        if (objetoRender == null)
        {
            TryGetComponent<MeshRenderer>(out objetoRender);
        }

        AtualizarVisual();
    }
}
