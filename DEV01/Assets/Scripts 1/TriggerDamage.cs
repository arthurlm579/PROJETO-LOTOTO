using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDamage : MonoBehaviour  
{
    public Heartsystem heart;
    public PlayerMovement player;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.KBCount = player.KBTime;
            if(collision.transform.position.x <= transform.position.x)
            {
                player.isKonock = true;
            }

            if (collision.transform.position.x > transform.position.x)
            {
                player.isKonock = false;
            }
            heart.vida--;
            player.anim.SetTrigger("TakeDamage");
        }
    }
}
