using UnityEngine;

public class JogadorInteracao : MonoBehaviour
{
    [Header("Configurações de Interação")]
    public float distanciaInteracao = 3f; // Distância máxima para conseguir tocar na válvula
    public KeyCode teclaInteracao = KeyCode.E; // Tecla para interagir

    private Camera cam;

    void Start()
    {
        // Encontra a câmera principal automaticamente pela tag
        cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("Câmera Principal (MainCamera) não foi encontrada na cena!");
        }
    }

    void Update()
    {
        // Verifica se o jogador apertou a tecla de interação
        if (Input.GetKeyDown(teclaInteracao))
        {
            TentarInteragir();
        }
    }

    void TentarInteragir()
    {
        if (cam == null) return;

        // Cria um raio a partir do centro da tela para a frente
        Ray raio = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Dispara o raio no espaço 3D
        if (Physics.Raycast(raio, out hit, distanciaInteracao))
        {
            // Verifica se o objeto atingido tem a tag "Valvula"
            if (hit.collider.CompareTag("Valvula"))
            {
                // Pega o script da válvula e ativa a interação
                ValvulaGas valvula = hit.collider.GetComponent<ValvulaGas>();
                if (valvula != null)
                {
                    valvula.Interagir();
                }
            }
        }
    }
}