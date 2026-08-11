using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<Heartsystem>() != null)
        {
            Heartsystem health = collider.GetComponent<Heartsystem>();
            health.vida--;
        }
    }
}
