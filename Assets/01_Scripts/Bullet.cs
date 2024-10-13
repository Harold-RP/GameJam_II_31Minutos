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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("El proyectil golpeó al enemigo.");
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Destruir el proyectil tras impactar
            Destroy(gameObject);
        }

        // Si el proyectil choca con el jugador
        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    Debug.Log("El proyectil golpeó al jugador.");
        //    Player player = collision.gameObject.GetComponent<Player>();
        //    if (player != null)
        //    {
        //        player.TakeDamage(damage);
        //    }

        //    // Destruir el proyectil tras impactar
        //    Destroy(gameObject);
        //}

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
        if (collision.gameObject.CompareTag("BossHead"))
        {
            Debug.Log("El proyectil golpeó a la cabeza.");           
            BossHead bossHead = collision.gameObject.GetComponent<BossHead>();
            if (bossHead != null)
            {             
                bossHead.TakeDamage(damage);
            }

            // Destruir el proyectil tras impactar
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Hand"))
        {
            Debug.Log("El proyectil golpeó a la Mano.");
            HeadAndHands hand = collision.gameObject.GetComponent<HeadAndHands>();
            if (hand != null)
            {
                hand.TakeDamage(damage);
            }

            // Destruir el proyectil tras impactar
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Head"))
        {
            Debug.Log("El proyectil golpeó a la Mano.");
            HeadAndHands head = collision.gameObject.GetComponent<HeadAndHands>();
            if (head != null)
            {
                head.TakeDamage(damage);
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
