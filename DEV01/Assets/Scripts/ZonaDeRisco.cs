using UnityEngine;

public class ZonaDeRisco : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.ProcessarEntradaNaZona();
        }
    }
}