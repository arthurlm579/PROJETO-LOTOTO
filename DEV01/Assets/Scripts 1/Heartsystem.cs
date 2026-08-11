using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Heartsystem : MonoBehaviour
{
    public int vida;
    public int vidaMaxima;

    public Image[] coracao;
    public Sprite cheio;
    public Sprite vazio;
    [SerializeField] private string NomeDaFaseDeMorte;
    
    
    
    void Start ()
    {

    }


    void Update ()
    {
        HealthLogic();
        DeadState();
    }

    void HealthLogic()
    {
        if(vida >vidaMaxima)
        {
            vida = vidaMaxima;
        }

        for (int i = 0; i < coracao.Length; i++)
        {
            if(i < vida)
            {
                coracao[i].sprite = cheio;
            }
            else
            {
                coracao[i].sprite = vazio;
            }


            if(i < vidaMaxima)
            {
                coracao [i].enabled = true;
            }
            else
            {
                coracao[i].enabled = false;
            }
        }
    }
    
    void DeadState()
    {
        if(vida <= 0)
        {
            GetComponent<PlayerMovement>().enabled = false;
            Destroy(gameObject,1.0f);
            SceneManager.LoadScene(NomeDaFaseDeMorte);
        }
    }
}
