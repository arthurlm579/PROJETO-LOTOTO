using UnityEngine;

public class KillOnTouch : MonoBehaviour
{
    [Header("Configurações de Morte")]
    [Tooltip("Se verdadeiro, o jogador volta para o início da fase atual")]
    public bool restartCurrentScene = true;

    [Tooltip("Se falso, apenas desativa o Player (útil para teste)")]
    public bool destroyPlayer = false;

    [Header("Mensagem de Morte")]
    public string deathMessage = "Você morreu!";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            KillPlayer(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            KillPlayer(other.gameObject);
        }
    }

    private void KillPlayer(GameObject player)
    {
        Debug.Log(deathMessage);

        if (destroyPlayer)
        {
            // Destrói o player completamente
            Destroy(player);
        }
        else if (restartCurrentScene)
        {
            // Reinicia a cena atual (mais comum em jogos de plataforma)
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
        else
        {
            // Apenas desativa o player (útil para debug)
            player.SetActive(false);
        }

        // Opcional: você pode chamar um efeito de câmera shake aqui
        // Camera.main.GetComponent<LiminalFirstPersonCamera>()?.TriggerScareShake(0.6f, 0.3f);
    }

    // Função pública caso queira matar o player de outro script
    public void KillPlayerNow()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            KillPlayer(player);
        }
    }
}