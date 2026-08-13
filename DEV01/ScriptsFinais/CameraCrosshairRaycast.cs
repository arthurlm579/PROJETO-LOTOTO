using UnityEngine;

public class CameraCrosshairRaycast : MonoBehaviour
{
    [Header("Configurações")]
    public Camera playerCamera;
    public float alcancedeInteracao = 3f;
    public LayerMask camadasInteragiveis;

    void Start()
    {
        // Trava o mouse no centro da tela e esconde o ponteiro
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Cria um raio exatamente no centro da tela (X: 0.5, Y: 0.5)
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Dispara o raio
        if (Physics.Raycast(ray, out hit, alcancedeInteracao, camadasInteragiveis))
        {
            // O raio atingiu algo!
            Debug.DrawLine(ray.origin, hit.point, Color.green);

            // Exemplo: Se apertar F no objeto mirado
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log($"Interagindo com: {hit.collider.name}");
            }
        }
        else
        {
            // O raio não atingiu nada
            Debug.DrawRay(ray.origin, ray.direction * alcancedeInteracao, Color.red);
        }
    }
}