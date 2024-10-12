using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//HOLA QUE TAL

public class Player : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpForce = 15f;
    public float meleeAttackRange = 1f;
    public float meleeAttackDamage = 10f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public Transform firePoint;
    public float maxHealth = 100f;   // Vida m�xima del jugador
    private float currentHealth;

    private Rigidbody2D rb;
    private bool isGrounded = false;

    public float jumpTimeMax = 0.3f;  // Tiempo m�ximo que puede durar el salto al mantener la tecla
    private float jumpTimeCounter;    // Contador de tiempo de salto
    private bool isJumping;

    public float fallMultiplier = 4.0f;      // Para ca�da r�pida
    public float lowJumpMultiplier = 3f;     // Para saltos cortos

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;  // Inicializar la vida actual
        rb.gravityScale = 2.5f;     // Escala de gravedad normal
    }

    void Update()
    {
        Move();
        Jump();
        HandleMeleeAttack();
        HandleRangedAttack();

        // Aplicar mayor gravedad durante la ca�da
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Aplicar gravedad extra cuando se suelta la tecla de salto
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 5f);  // Ajusta el valor para la rapidez de ca�da

        }

        // Debug para ver la vida
        Debug.Log("Player Health: " + currentHealth);
    }

    // M�todo para mover al jugador
    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (moveInput < 0)
        {
            firePoint.localPosition = new Vector3(-1f, 0f, 0f);
        }
        else if (moveInput > 0)
        {
            firePoint.localPosition = new Vector3(1f, 0f, 0f);
        }
    }

    void Jump()
{
    if (Input.GetKeyDown(KeyCode.W) && isGrounded)  // Cambi� a KeyCode.W
    {
        isJumping = true;
        jumpTimeCounter = jumpTimeMax;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    // Si se sigue presionando el bot�n de salto y a�n no ha llegado al tiempo m�ximo
    if (Input.GetKey(KeyCode.W) && isJumping)  // Cambi� a KeyCode.W
    {
            if (jumpTimeCounter > 0)
            {
                // Disminuir la fuerza de salto al mantener presionada la tecla
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.7f); // Ajusta 0.5f a un valor que sientas correcto
                jumpTimeCounter -= Time.deltaTime;  // Restar tiempo mientras se presiona el bot�n
            }
            else
            {
                isJumping = false;
            }
        }

    // Si el jugador suelta el bot�n de salto, se interrumpe el salto
    if (Input.GetKeyUp(KeyCode.W))  // Cambi� a KeyCode.W
    {
        isJumping = false;
    }
}


    // M�todo para manejar el ataque cuerpo a cuerpo
    void HandleMeleeAttack()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(firePoint.position, meleeAttackRange);

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Boss"))
                {
                    Debug.Log("Golpeaste a " + enemy.name);
                    // Hacer da�o al enemigo (suponiendo que tenga un m�todo TakeDamage)
                    enemy.GetComponent<Boss>().TakeDamage(meleeAttackDamage);
                }
            }
        }
    }

    void HandleRangedAttack()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector2 direction = firePoint.localPosition.x > 0 ? Vector2.right : Vector2.left;
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            projectileRb.velocity = direction * projectileSpeed;
            Debug.Log("Disparaste un proyectil hacia " + direction);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }

    // M�todo para recibir da�o
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // M�todo para la muerte del jugador
    void Die()
    {
        Debug.Log("Jugador ha muerto.");
        // Aqu� podr�as implementar la l�gica de muerte (reiniciar el nivel, mostrar pantalla de derrota, etc.)
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, meleeAttackRange);
        }
    }
}
