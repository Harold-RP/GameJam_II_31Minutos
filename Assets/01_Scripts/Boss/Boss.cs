using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public float chargeSpeed = 5f;        // Velocidad de embestida
    public float jumpForce = 10f;         // Fuerza del salto
    public float wallBounceSpeed = 3f;    // Velocidad al rebotar contra la pared
    public float attackCooldown = 2f;     // Tiempo de descanso entre ataques
    public float minChargeDistance = 5f;  // Distancia m�nima para embestir
    public float maxJumpDistance = 8f;    // Distancia m�xima para saltar

    private Transform player;
    private Rigidbody2D rb;
    private bool isCharging = false;
    private bool canJump = true;
    private bool isGrounded = false;      // Controla si est� en el suelo
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

    // L�gica para decidir y ejecutar un ataque
    IEnumerator DecideAndPerformAttack()
    {
        canAttack = false;

        // Calcular la distancia al jugador
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Condici�n para elegir embestida o salto
        if (distanceToPlayer < minChargeDistance)
        {
            Debug.Log("El jefe est� cerca, preparando embestida.");
            ChargeAtPlayer();  // Priorizar embestida si est� muy cerca
        }
        else if (distanceToPlayer < maxJumpDistance && canJump)
        {
            Debug.Log("El jefe est� a una distancia adecuada, preparando salto.");
            StartCoroutine(JumpTowardsPlayer());  // Priorizar salto si est� a una distancia media
        }
        else
        {
            Debug.Log("El jefe est� lejos, decidiendo embestida.");
            ChargeAtPlayer();  // Si est� lejos, intentar embestir
        }

        // Esperar antes de realizar otro ataque
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        Debug.Log("El jefe est� listo para otro ataque.");
    }

    // M�todo para embestir al jugador
    void ChargeAtPlayer()
    {
        isCharging = true;

        // Calcular la direcci�n hacia el jugador
        Vector2 direction = (player.position - transform.position).normalized;

        // Aplicar velocidad directamente en la direcci�n del jugador
        rb.velocity = new Vector2(direction.x * chargeSpeed, rb.velocity.y);

        Debug.Log("El jefe est� embistiendo al jugador en la direcci�n: " + direction);
    }

    // M�todo para saltar hacia el jugador
    IEnumerator JumpTowardsPlayer()
    {
        if (isGrounded)  // Solo saltar si est� en el suelo
        {
            canJump = false;
            yield return new WaitForSeconds(attackCooldown);

            Vector2 jumpDirection = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(jumpDirection.x * jumpForce, jumpForce);

            Debug.Log("El jefe ha saltado hacia el jugador en la direcci�n: " + jumpDirection);
            canJump = true;
        }
        else
        {
            Debug.Log("El jefe intent� saltar, pero no est� en el suelo.");
        }
    }

    // M�todo para detectar la colisi�n con las paredes
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Rebota hacia atr�s al chocar con una pared
            rb.velocity = new Vector2(-rb.velocity.x * wallBounceSpeed, rb.velocity.y);
            isCharging = false;

            Debug.Log("El jefe choc� contra una pared y rebot�.");
        }

        // Detectar si toca el suelo
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
            Debug.Log("El jefe est� tocando el suelo.");
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
