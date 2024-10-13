using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public float maxHealth = 3f;   // Vida máxima del jugador
    private float currentHealth;

    private Rigidbody2D rb;
    private bool isGrounded = false;

    public float jumpTimeMax = 0.3f;  // Tiempo máximo que puede durar el salto al mantener la tecla
    private float jumpTimeCounter;    // Contador de tiempo de salto
    private bool isJumping;

    public float fallMultiplier = 4.0f;      // Para caída rápida
    public float lowJumpMultiplier = 3f;     // Para saltos cortos

    public Animator animator;
    public RectTransform speedUpImg;
    public RectTransform speedDownImg;
    public RectTransform lifeUpImg;
    public RectTransform lifeDownImg;
    public Animator fadeAnim;


    public GameObject lifePrefab;  // Prefab de la imagen de vida (ícono)
    public Transform livesPanel;   // El panel que contendrá las imágenes de vidas

    private List<GameObject> lifeIcons = new List<GameObject>();



    public AudioSource audioSource;   // Referencia al AudioSource
    public AudioClip shootSound;
    public AudioClip knifeSound;
    public AudioClip jump;
    public AudioClip fall;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;  // Inicializar la vida actual
        rb.gravityScale = 2.5f;     // Escala de gravedad normal
        UpdateLivesUI();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();  // Obtiene el AudioSource del objeto si no está asignado
        }

    }

    public void LoseLife()
    {
        if (maxHealth > 0)
        {
            maxHealth--;
            UpdateLivesUI();
        }
    }

    public void GainLife()
    {
        maxHealth++;
        UpdateLivesUI();
    }

    void UpdateLivesUI()
    {
        // Limpiar las imágenes actuales de vidas
        foreach (GameObject icon in lifeIcons)
        {
            Destroy(icon);
        }
        lifeIcons.Clear();

        // Añadir tantas imágenes como vidas tenga el jugador
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject lifeIcon = Instantiate(lifePrefab, livesPanel);

            // Ajustar la posición de cada imagen según su índice
            RectTransform rectTransform = lifeIcon.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(i * 50, 0);  // Separar las imágenes con un espacio de 50 unidades
            lifeIcons.Add(lifeIcon);
        }
    }


    void Update()
    {
        Move();
        Jump();
        HandleMeleeAttack();
        HandleRangedAttack();

        // Aplicar mayor gravedad durante la caída
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
            audioSource.PlayOneShot(fall);
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 5f);  // Ajusta el valor para la rapidez de caída

            animator.SetTrigger("fall");

        }

        // Debug para ver la vida
        Debug.Log("Player Health: " + currentHealth);
    }

    // Método para mover al jugador
    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Control del movimiento horizontal del jugador
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Si el jugador se está moviendo a la izquierda
        if (moveInput < 0)
        {
            // Voltea el jugador hacia la izquierda
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        // Si el jugador se está moviendo a la derecha
        else if (moveInput > 0)
        {
            // Voltea el jugador hacia la derecha
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        // Control de la animación (usamos el valor absoluto de moveInput)
        animator.SetFloat("mov", Mathf.Abs(moveInput));
    }



    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)  // Cambié a KeyCode.W
        {
            audioSource.PlayOneShot(jump);
            isJumping = true;
            jumpTimeCounter = jumpTimeMax;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Si se sigue presionando el botón de salto y aún no ha llegado al tiempo máximo
        if (Input.GetKey(KeyCode.W) && isJumping)  // Cambié a KeyCode.W
        {
            if (jumpTimeCounter > 0)
            {
                // Disminuir la fuerza de salto al mantener presionada la tecla
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.7f); // Ajusta 0.5f a un valor que sientas correcto
                jumpTimeCounter -= Time.deltaTime;  // Restar tiempo mientras se presiona el botón
            }
            else
            {
                isJumping = false;
            }
        }

        // Si el jugador suelta el botón de salto, se interrumpe el salto
        if (Input.GetKeyUp(KeyCode.W))  // Cambié a KeyCode.W
        {
            isJumping = false;
        }
    }


    // Método para manejar el ataque cuerpo a cuerpo
    private bool canAttack = true;

    void HandleMeleeAttack()
    {
        if (canAttack && Input.GetKeyDown(KeyCode.K))
        {
            audioSource.PlayOneShot(knifeSound);
            StartCoroutine(PerformMeleeAttack());
        }
    }

    IEnumerator PerformMeleeAttack()
    {
        // Evitar que el jugador ataque de inmediato de nuevo
        canAttack = false;

        // Activar la animación de ataque
        animator.SetTrigger("attack0");

        // Detectar los enemigos en el rango de ataque
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(firePoint.position, meleeAttackRange);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Boss"))
            {
                Debug.Log("Golpeaste a " + enemy.name);
                enemy.GetComponent<Boss>().TakeDamage(meleeAttackDamage);
            }

            if (enemy.CompareTag("Enemy"))
            {
                Debug.Log("Golpeaste a " + enemy.name);
                enemy.GetComponent<Enemy>().TakeDamage(meleeAttackDamage);
            }

            if (enemy.CompareTag("BossHead"))
            {
                Debug.Log("Golpeaste a " + enemy.name);
                enemy.GetComponent<BossHead>().TakeDamage(meleeAttackDamage);
            }

            if (enemy.CompareTag("Hand"))
            {
                Debug.Log("Golpeaste a " + enemy.name);

                // Hacer daño al enemigo (suponiendo que tenga un método TakeDamage)
                enemy.GetComponent<HeadAndHands>().TakeDamage(meleeAttackDamage);
            }

            if (enemy.CompareTag("Head"))
            {
                Debug.Log("Golpeaste a " + enemy.name);

                // Hacer daño al enemigo (suponiendo que tenga un método TakeDamage)
                enemy.GetComponent<HeadAndHands>().TakeDamage(meleeAttackDamage);
            }
        }

        // Esperar el tiempo del cooldown antes de permitir otro ataque
        yield return new WaitForSeconds(0.2f);

        // Permitir atacar de nuevo
        canAttack = true;
    }



    void HandleRangedAttack()
    {

        

        if (Input.GetKeyDown(KeyCode.L)) // Suponiendo que la tecla L es para disparar
        {
            audioSource.PlayOneShot(shootSound);
            // Activar la animación de ataque usando un Trigger
            animator.SetTrigger("attack1"); // Cambia "attack" por el nombre de tu Trigger en el Animator

            // Instanciar el proyectil en la posición del firePoint
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            // Obtener la dirección de disparo según la escala del jugador
            Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            // Aplicar velocidad al proyectil
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
            animator.SetBool("ground", true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
            animator.SetBool("ground", false);
        }
    }

    // Método para recibir daño
    public void TakeDamage(float damage)
    {

        currentHealth -= damage;
        LoseLife();
        UpdateLivesUI();

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    // Método para la muerte del jugador
    IEnumerator Die()
    {
        Debug.Log("Jugador ha muerto.");
        AsyncOperation operation = SceneManager.LoadSceneAsync("GameOver");
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                fadeAnim.SetTrigger("FadeOut");
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public void UsePowerUp(PowerUp powerUp)
    {
        switch (powerUp.type)
        {
            case PowerUpType.Life:
                if (Random.Range(0, 2) == 0)
                {
                    currentHealth -= powerUp.value;
                    LoseLife();
                    UpdateLivesUI();
                    StartCoroutine(showPowerUpImg(lifeDownImg));
                }
                else
                {
                    currentHealth += powerUp.value;
                    GainLife();
                    UpdateLivesUI();
                    StartCoroutine(showPowerUpImg(lifeUpImg));
                }
                break;

            case PowerUpType.Speed:
                if(Random.Range(0, 2) == 0)
                {
                    moveSpeed -= powerUp.value;
                    StartCoroutine(showPowerUpImg(speedDownImg));
                }
                else
                {
                    moveSpeed += powerUp.value;
                    StartCoroutine(showPowerUpImg(speedUpImg));
                }
                break;
        }
    }

    IEnumerator showPowerUpImg(RectTransform img)
    {
        LeanTween.alpha(img, 1f, 1f);
        yield return new WaitForSeconds(3f);
        LeanTween.alpha(img, 0, 1f);
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