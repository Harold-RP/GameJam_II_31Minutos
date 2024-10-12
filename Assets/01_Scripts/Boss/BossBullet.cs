using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet : MonoBehaviour
{

    public float speed = 10f;             // Velocidad inicial de la bala
    public float lifeTime = 5f;           // Tiempo antes de que la bala se destruya autom�ticamente
    public float damage = 20f;            // Da�o que inflige la bala
    public PhysicsMaterial2D bounceMaterial; // Material f�sico para controlar el rebote
    private Rigidbody2D rb;

    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Asignar el material f�sico para controlar el rebote
        rb.sharedMaterial = bounceMaterial;

        // Aplicar la velocidad inicial a la bala
        rb.velocity = transform.right * speed;

        // Destruir la bala despu�s de un tiempo
        StartCoroutine(DestroyAfterTime());
    }

    void Update()
    {
        if (player != null)
        {
            // Calcula la direcci�n hacia el jugador
            Vector2 direction = (player.position - transform.position).normalized;

            // Mueve la bala en la direcci�n del jugador
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si la bala choca con una pared o el suelo, ajustar la direcci�n y mantener la velocidad
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
        {
            Vector2 reflectDir = Vector2.Reflect(rb.velocity, collision.contacts[0].normal);

            // Ajustar la direcci�n reflejada y mantener la velocidad constante
            rb.velocity = reflectDir.normalized * speed;
        }

        // Si la bala choca con el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerHealth = collision.gameObject.GetComponent<Player>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            // Destruir la bala tras impactar
            Destroy(gameObject);
        }

        // Si la bala choca con un proyectil del jugador, destruirla
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Boss"))
        {
            // Aqu� no haces nada, lo que permite que la bala atraviese al jefe.
            return;
        }
    }

    IEnumerator DestroyAfterTime()
    {
        // Destruir la bala despu�s de un tiempo
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }


}
