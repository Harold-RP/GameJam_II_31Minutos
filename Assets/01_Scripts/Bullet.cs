using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3f;       // Tiempo de vida del proyectil antes de desaparecer
    public float damage = 10f;        // Daño que inflige el proyectil

    void Start()
    {
        // Destruir el proyectil después de un tiempo para evitar que siga volando indefinidamente
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si el proyectil choca con un enemigo
        if (collision.gameObject.CompareTag("Boss"))
        {
            Debug.Log("El proyectil golpeó a " + collision.gameObject.name);
            // Lógica para hacerle daño al enemigo
            Boss enemyHealth = collision.gameObject.GetComponent<Boss>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Destruir el proyectil tras impactar
            Destroy(gameObject);
        }

        // Si el proyectil choca con el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("El proyectil golpeó al jugador.");
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            // Destruir el proyectil tras impactar
            Destroy(gameObject);
        }

        // Si el proyectil choca con el jefe
        if (collision.gameObject.CompareTag("Boss"))
        {
            Debug.Log("El proyectil golpeó al jefe.");
            Boss boss = collision.gameObject.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }

            // Destruir el proyectil tras impactar
            Destroy(gameObject);
        }

        // Si el proyectil choca con cualquier otro objeto (como una pared), también se destruye
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
    }
}
