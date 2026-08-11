using UnityEngine;

public class TriiggerDamageEnemy : MonoBehaviour
{
    // Não precisamos mais da referência pública 'heart'
    public float damageAmount = 1; // Adicionei a quantidade de dano para ser mais claro

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Verifica se acertamos um inimigo
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 2. Tenta encontrar o script HeartEnemy no objeto que foi atingido (collision.gameObject)
            HeartEnemy targetHealth = collision.gameObject.GetComponent<HeartEnemy>();

            // Verificação de segurança (verifica se o script foi encontrado)
            if (targetHealth != null)
            {
                // 3. Aplica o dano no objeto atingido
                targetHealth.TakeDamage((int)damageAmount);

                // Sugestão: Adicione um Destroy(gameObject); aqui se for um projétil
            }
        }
    }
}