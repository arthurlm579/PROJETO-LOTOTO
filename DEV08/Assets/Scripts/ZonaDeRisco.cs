using UnityEngine;

public class ZonaDeRisco : MonoBehaviour
{
    [Header("Configurações da Zona")]
    [SerializeField] private GameManager gameManager;

    // Aqui você arrasta o script de energia específico desta zona (ex: EnergiaEletrica)
    [SerializeField] private EnergiaBase energiaDestaZona;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem entrou na zona foi o jogador
        if (other.CompareTag("Player"))
        {
            // Pede para o GameManager validar especificamente esta energia
            gameManager.ProcessarEntradaNaZona(energiaDestaZona);
        }
    }
}