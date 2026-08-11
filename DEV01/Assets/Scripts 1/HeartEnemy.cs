using UnityEngine;

public class HeartEnemy : MonoBehaviour
{
    // A variável 'vida' não precisa ser pública (public), mas mantive para compatibilidade
    public int vida;
    public int vidaMaxima = 3; // Bom ter um valor padrão

    void Start()
    {
        // 1. Inicialização correta
        vida = vidaMaxima;
    }

    // REMOVEMOS O MÉTODO UPDATE() para otimizar!

    // Função pública chamada pelo script de ataque do Player
    public void TakeDamage(int damageAmount)
    {
        vida -= damageAmount;

        // 2. Agora, verificamos a lógica SÓ quando leva dano
        HealthLogic();
        DeadState();

        // Opcional: Adicione aqui feedback visual/sonoro (som de hit, cor piscando, etc.)
    }

    void HealthLogic()
    {
        // Limita a vida para que não ultrapasse o máximo
        if (vida > vidaMaxima)
        {
            vida = vidaMaxima;
        }
    }

    void DeadState()
    {
        if (vida <= 0)
        {
            // Tenta encontrar o gerenciador de pontos
            Point scoreManager = FindAnyObjectByType<Point>(); // Usando a versão mais compatível

            if (scoreManager != null)
            {
                // Mensagem CRÍTICA de Sucesso
                Debug.Log("? Gerenciador de Pontos Encontrado! Tentando Adicionar Ponto.");
                scoreManager.AddScore(1);
            }
            else
            {
                // Mensagem CRÍTICA de Falha (Se esta aparecer, o problema é CONFIGURAÇÃO da cena)
                Debug.LogError("? ERRO FATAL: Script 'Point' NÃO FOI ENCONTRADO na cena. Pontuação não será adicionada.");
            }

            GetComponent<EnemyIA>().enabled = false;
            Destroy(gameObject, 0.1f);
        }
    }
}