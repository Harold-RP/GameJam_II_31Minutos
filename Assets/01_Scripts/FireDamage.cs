using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDamage : MonoBehaviour
{
    public float damage = 10f; // Daño que hará al jugador
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el objeto que entra en el fuego es el jugador
        if (collision.CompareTag("Player"))
        {
            // Obtener el componente del jugador y aplicar daño
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}