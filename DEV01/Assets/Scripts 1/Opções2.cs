using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Opções2 : MonoBehaviour
{
    [SerializeField] private GameObject painelOpcoes2;
    [SerializeField] private string NomeDeLevelDeJogo2;
    [SerializeField] private string NomeDeFaseDeMorte;

   
    public void AbrirOpcoes()
    {
        painelOpcoes2.SetActive(true);
    }

    public void FecharOpcoes()
    {
        painelOpcoes2.SetActive(false);
    }
    public void Jogar()
    {
        SceneManager.LoadScene(NomeDeLevelDeJogo2);
    }
    public void VoltarJogo()
    {
       
    }
}
