using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private int damage = 1;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<HeartEnemy>() != null)
        {
            HeartEnemy health = collider.GetComponent<HeartEnemy>();
            health.vidaMaxima --;   
        }
    }
}
