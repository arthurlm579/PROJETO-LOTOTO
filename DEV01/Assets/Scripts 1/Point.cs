using UnityEngine;
using UnityEngine.UI;

public class Point : MonoBehaviour
{
    public Image[] Skull;
    // Removi a atribuição 10 aqui, pois o limite será Skull.Length
    public int PontuacaoMax = 10;
    public int Pontuacao = 0;
    public Sprite ponto;

    void Start()
    {
        Pontuacao = 0;
        // Chama no Start para garantir que as caveiras comecem escondidas
        UpdateDisplay();
    }

    public void AddScore(int points)
    {
        int pontuacaoAntes = Pontuacao;

        // 1. Adiciona a Pontuação
        Pontuacao += points;

        // 2. ? CORREÇÃO 1: Limita a pontuação AQUI (local correto)
        if (Pontuacao > Skull.Length) // Uso Skull.Length para limite visual
        {
            Pontuacao = Skull.Length;
        }

        Debug.Log($"Pontuação Adicionada! Antes: {pontuacaoAntes}, Depois: {Pontuacao}");

        // 3. ? CORREÇÃO 2: Chama a atualização visual AQUI!
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        for (int i = 0; i < Skull.Length; i++)
        {
            // O limite já foi tratado em AddScore, então a lógica fica limpa:
            if (i < Pontuacao)
            {
                // Ativa a imagem se o índice for menor que a pontuação atual
                Skull[i].enabled = true;
                // Se quiser trocar a imagem para "ponto", use:
                // Skull[i].sprite = ponto; 
            }
            else
            {
                // Desativa a imagem
                Skull[i].enabled = false;
            }
        }
    }
}