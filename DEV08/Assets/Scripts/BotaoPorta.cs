using UnityEngine;

public class BotaoPorta : MonoBehaviour
{
    [Header("Conexão com a Porta")]
    [Tooltip("Arraste o GameObject da Porta aqui")]
    public GameObject portaObjeto;

    [Header("Configuração do Raycast")]
    public float distanciaInteracao = 3.0f;
    public KeyCode teclaInteracao = KeyCode.F;

    private Camera cameraPrincipal;
    private PortaElevadica scriptPorta;

    void Start()
    {
        cameraPrincipal = Camera.main;

        // Tenta buscar o script na porta automaticamente
        if (portaObjeto != null)
        {
            scriptPorta = portaObjeto.GetComponent<PortaElevadica>();
        }
    }

    void Update()
    {
        if (cameraPrincipal == null || scriptPorta == null) return;

        Ray ray = new Ray(cameraPrincipal.transform.position, cameraPrincipal.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaInteracao))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                if (InteractionUI.Instance != null)
                {
                    InteractionUI.Instance.Mostrar("Pressione <color=green>[F]</color> para acionar a Porta");
                }

                if (Input.GetKeyDown(teclaInteracao))
                {
                    scriptPorta.AlternarPorta();
                }
            }
        }
    }
}