using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float chargeSpeed = 5f;        // Velocidad de embestida
    public float jumpForce = 10f;         // Fuerza del salto
    public float wallBounceSpeed = 3f;    // Velocidad al rebotar contra la pared
    public float attackCooldown = 2f;     // Tiempo de descanso entre ataques
    public float minChargeDistance = 5f;  // Distancia mínima para embestir
    public float maxJumpDistance = 8f;    // Distancia máxima para saltar

    private Transform player;
    private Rigidbody2D rb;
    private bool isCharging = false;
    private bool canJump = true;
    private bool isGrounded = false;      // Controla si está en el suelo
    private bool canAttack = true;        // Controla si puede atacar

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Detectar al jugador usando su tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        Debug.Log("Boss inicializado. Jugador detectado.");
    }

    void Update()
    {
        if (player == null) return; // Si no hay jugador, no hacer nada

        if (canAttack)
        {
            StartCoroutine(DecideAndPerformAttack());
        }
    }

    // Lógica para decidir y ejecutar un ataque
    IEnumerator DecideAndPerformAttack()
    {
        canAttack = false;

        // Calcular la distancia al jugador
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Condición para elegir embestida o salto
        if (distanceToPlayer < minChargeDistance)
        {
            Debug.Log("El jefe está cerca, preparando embestida.");
            ChargeAtPlayer();  // Priorizar embestida si está muy cerca
        }
        else if (distanceToPlayer < maxJumpDistance && canJump)
        {
            Debug.Log("El jefe está a una distancia adecuada, preparando salto.");
            StartCoroutine(JumpTowardsPlayer());  // Priorizar salto si está a una distancia media
        }
        else
        {
            Debug.Log("El jefe está lejos, decidiendo embestida.");
            ChargeAtPlayer();  // Si está lejos, intentar embestir
        }

        // Esperar antes de realizar otro ataque
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        Debug.Log("El jefe está listo para otro ataque.");
    }

    // Método para embestir al jugador
    void ChargeAtPlayer()
    {
        isCharging = true;

        // Calcular la dirección hacia el jugador
        Vector2 direction = (player.position - transform.position).normalized;

        // Aplicar velocidad directamente en la dirección del jugador
        rb.velocity = new Vector2(direction.x * chargeSpeed, rb.velocity.y);

        Debug.Log("El jefe está embistiendo al jugador en la dirección: " + direction);
    }

    // Método para saltar hacia el jugador
    IEnumerator JumpTowardsPlayer()
    {
        if (isGrounded)  // Solo saltar si está en el suelo
        {
            canJump = false;
            yield return new WaitForSeconds(attackCooldown);

            Vector2 jumpDirection = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(jumpDirection.x * jumpForce, jumpForce);

            Debug.Log("El jefe ha saltado hacia el jugador en la dirección: " + jumpDirection);
            canJump = true;
        }
        else
        {
            Debug.Log("El jefe intentó saltar, pero no está en el suelo.");
        }
    }

    // Método para detectar la colisión con las paredes
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Rebota hacia atrás al chocar con una pared
            rb.velocity = new Vector2(-rb.velocity.x * wallBounceSpeed, rb.velocity.y);
            isCharging = false;

            Debug.Log("El jefe chocó contra una pared y rebotó.");
        }

        // Detectar si toca el suelo
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            Debug.Log("El jefe está tocando el suelo.");
        }
    }

    // Detectar cuando deja de estar en el suelo
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
            Debug.Log("El jefe ha dejado de estar en el suelo.");
        }
    }
}
