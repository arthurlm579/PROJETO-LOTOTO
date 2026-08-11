using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

    public class MenuPrincipalManage : MonoBehaviour
{
   [SerializeField] private string NomeDeLevelDeJogo;
   [SerializeField] private GameObject painelMenuInicial;
   [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelControles;

public void Jogar()
    {
        SceneManager.LoadScene(NomeDeLevelDeJogo);
    }

    public void AbrirOpcoes()
    {
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
    }
    
    public void AbrirControles()
    {
        painelOpcoes.SetActive(false);
        painelControles.SetActive(true);
    }
    
    public void FecaharControles()
    {
        painelControles.SetActive(false) ;
        painelOpcoes.SetActive(true) ;
    }
    public void FecharOpcoes()
    {
        painelOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

    public void SairJogo()
    {
        Debug.Log("Sair do Jogo");
        Application.Quit();
        }
}
