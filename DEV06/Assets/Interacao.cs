using UnityEngine;

public class Interacao : MonoBehaviour
{
    public float distanciaDoRaio = 3f;

    void Update()
    {
        // Detecta quando você aperta a tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            VerificarInteracao();
        }
    }

    void VerificarInteracao()
    {
        RaycastHit hit;
        // Lança um raio para frente para ver se há um cubo de energia
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanciaDoRaio))
        {
            // Tenta pegar o script Energia do objeto atingido
            EnergiaHidraulica scriptEnergia = hit.collider.GetComponent<EnergiaHidraulica>();

            if (scriptEnergia != null)
            {
                // CHAMA A FUNÇÃO CERTA:
                // Se estiver ligado, ele inicia o delay do outro script
                //scriptEnergia.AlternarEnergia();
            }
        }
    }
}