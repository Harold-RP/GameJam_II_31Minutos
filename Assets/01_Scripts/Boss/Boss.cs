using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Boss : MonoBehaviour
{
    [Header("---------------------- Stats ------------------------")]
    public float chargeSpeed = 5f;        // Velocidad de embestida
    public float jumpForce = 10f;         // Fuerza del salto
    public float wallBounceSpeed = 3f;    // Velocidad al rebotar contra la pared
    public float attackCooldown = 2f;     // Tiempo de descanso entre ataques
    public float minChargeDistance = 5f;  // Distancia m�nima para embestir
    public float maxJumpDistance = 8f;    // Distancia m�xima para saltar
    private bool isGrounded = false;      // Controla si est� en el suelo
    private bool canAttack = true;        // Controla si puede atacar
    private bool isCharging = false;
    private bool canJump = true;
    public float meleeRange = 2f;            // Radio para el ataque cuerpo a cuerpo
    public float meleeDamage = 20f;          // Daño del ataque cuerpo a cuerpo
    public float slowSpeed = 2f;             // Velocidad lenta al acercarse al jugador
    public float fireAttackDuration = 3f;    // Duración del ataque de fuego
    private float fireAttackCooldown = 10f;   // Tiempo de espera entre ataques de fuego
    private float lastFireAttackTime = 0f;    // Última vez que se realizó el ataque de fuego
    public float Phase4Speed = 5f;
    public float fireActivationRange = 5f;


    public float laserDuration = 0.5f;
    public int bossPhase = 1;
    bool isAttacking = false;

    public float maxHealth = 200f; 
    private float currentHealth;
    bool isHeadSpawned = false;

    [Header("---------------------- References ------------------------")]
    public GameObject laserPrefab;
    public GameObject headPrefab;
    public Transform firePoint;
    private Transform player;
    private Rigidbody2D rb;
    public FireSpawn[] fireSpawns;
    public GameObject bigBulletPrefab;
    GameObject bossHeadGO;
    CapsuleCollider2D collider;
    public Transform[] corners;
    public GameObject bossPrefab;
    public GameObject handPrefab;
    public GameObject BossheadPrefab;
    public Transform[] spawnPoints;

    void Start()
    {

        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
       collider = GetComponent<CapsuleCollider2D>();


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
        if (canAttack && !isAttacking)
        {
            switch (bossPhase)
            {
                case 1:
                    StartCoroutine(DecideAndPerformAttack());
                    break;
                case 2:
                    TeleportToRandomCorner();
                    Mirror();
                    AimAtPlayer();
                    StartCoroutine(LaserAttack());
                    break;
                case 3:
                    SpawnHead();
                    break;
                case 4:
                    if (Time.time - lastFireAttackTime >= fireAttackCooldown)
                    {
                        StartCoroutine(FireWallAttack());  // Ejecuta el ataque de fuego si el cooldown ha terminado
                        lastFireAttackTime = Time.time;    // Actualiza el tiempo del último ataque
                    }
                    else
                    {
                        MoveTowardsPlayer();  // Mientras tanto, el jefe se mueve hacia el jugador
                    }
                    break;
                case 5:
                    Mirror();
                    AimAtPlayer();
                    StartCoroutine(Phase5Attack());

                    break;
            }

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

    void Mirror()
    {
        if (player.position.x < transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void AimAtPlayer()
    {
        Vector2 dir = player.position - transform.position;
        float angleZ = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, -angleZ);
    }

    IEnumerator LaserAttack()
    {
        isAttacking = true;
        canAttack = false;
        float attackTimer = 0f;
        float laserInterval = 0.03f; // Intervalo entre disparos de láser

        while (attackTimer < laserDuration)
        {
            Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
            yield return new WaitForSeconds(laserInterval);
            attackTimer += laserInterval;
        }

        // Esperar antes de poder realizar otro ataque
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        isAttacking = false;
        Debug.Log("El jefe está listo para otro ataque.");
    }

    void TeleportToRandomCorner()
    {
        int randomIndex = Random.Range(0, corners.Length);
        transform.position = corners[randomIndex].position;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            CheckPhaseChange();
        }
    }

    void Die()
    {
        Debug.Log("El jefe ha sido derrotado.");
        GameObject.Destroy(gameObject);
    }


    //Phase 3 ------------------------------------------------------------------------
    void SpawnHead()
    {
        if (!isHeadSpawned) // Si la cabeza no ha sido generada aún
        {
            isHeadSpawned = true;
            collider.enabled = false;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            bossHeadGO = Instantiate(headPrefab, transform.position, transform.rotation);
        }
        else // Cabeza ya generada, revisar si sigue viva
        {
            if (bossHeadGO == null) // La cabeza ha sido destruida
            {
                isHeadSpawned = false;
                rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY; // Liberar restricción en Y
                collider.enabled = true;
                bossPhase = 4; // Pasar a la siguiente fase
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            else
            {
                // Aquí puedes agregar la animación de idle u otra lógica si la cabeza aún existe.
                BossHead bossHead = bossHeadGO.GetComponent<BossHead>();
                if (bossHead == null)
                {
                    // En caso de que haya problemas con el componente (aunque esto debería cubrirse con el chequeo anterior)
                    isHeadSpawned = false;
                    rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
                    collider.enabled = true;
                    bossPhase = 4;
                }
            }
        }
    }




    //Phase 4 ------------------------------------------------------------------------
    IEnumerator Phase4Behaviour()
    {
        canAttack = false;

        // Movimiento lento hacia el jugador
        MoveTowardsPlayer(slowSpeed);

        // Si está dentro del rango de ataque cuerpo a cuerpo
        if (Vector2.Distance(transform.position, player.position) <= meleeRange)
        {
            MeleeAttack();
        }

        // Realizar ataque de fuego desde las paredes/suelo
        StartCoroutine(FireWallAttack());

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }



    void MoveTowardsPlayer(float speed)
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
    }
    void MeleeAttack()
    {
        Debug.Log("El jefe realiza un ataque cuerpo a cuerpo.");

        // Si el jugador está dentro del rango de ataque
        if (Vector2.Distance(transform.position, player.position) <= meleeRange)
        {
            Player playerHealth = player.GetComponent<Player>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(meleeDamage);
            }
        }
    }

    IEnumerator FireWallAttack()
    {
        Debug.Log("El jefe activa el ataque de fuego desde las paredes y el suelo.");

        isAttacking = true;
        canAttack = false; // El jefe no puede moverse ni atacar mientras dure el ataque de fuego.

        // Generar fuego en cada punto de spawn usando el método ActivateFire de cada FireSpawn
        foreach (FireSpawn fireSpawn in fireSpawns)
        {
            fireSpawn.ActivateFire();
        }

        // El fuego dura 3 segundos (ya controlado en cada FireSpawn)
        yield return new WaitForSeconds(fireAttackDuration);

        isAttacking = false;
        canAttack = true;
        Debug.Log("El ataque de fuego ha terminado.");
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * Phase4Speed, rb.velocity.y);  // Ajusta `moveSpeed` a la velocidad que desees
        Debug.Log("El jefe se mueve hacia el jugador.");
    }

    //Phase 5 ------------------------------------------------------------------------
    IEnumerator Phase5Attack()
    {
 
        // Aquí puedes detener cualquier movimiento del jefe
        yield return new WaitForSeconds(8f);

        // Eliminar el prefab actual
        Destroy(gameObject);

        // Instanciar las nuevas partes
        GameObject hand1 = Instantiate(handPrefab, spawnPoints[0].position, Quaternion.identity);
        GameObject hand2 = Instantiate(handPrefab, spawnPoints[1].position, Quaternion.identity);
        GameObject head = Instantiate(BossheadPrefab, spawnPoints[2].position, Quaternion.identity);

        // Asignar la vida compartida a cada parte
        hand1.GetComponent<HeadAndHands>().SetSharedLife(currentHealth);
        hand2.GetComponent<HeadAndHands>().SetSharedLife(currentHealth);
        head.GetComponent<HeadAndHands>().SetSharedLife(currentHealth);


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



//  Cambio de fase

    void CheckPhaseChange()
    {
        // Cada 100 de vida cambiamos de fase
        if (currentHealth <= 400 && bossPhase == 1)
        {
            StartCoroutine(ChangePhase(2));
        }
        else if (currentHealth <= 300 && bossPhase == 2)
        {
            StartCoroutine(ChangePhase(3));
            rb.isKinematic = false;
        }
        else if (currentHealth <= 200 && bossPhase == 3)
        {
            rb.freezeRotation = true;
            StartCoroutine(ChangePhase(4));
        }
        else if (currentHealth <= 100 && bossPhase == 4)
        {
            StartCoroutine(ChangePhase(5));
        }
    }

    IEnumerator ChangePhase(int newPhase)
    {
        // El jefe se queda quieto durante 4 segundos antes de cambiar de fase
        canAttack = false; // Deshabilita los ataques
        rb.velocity = Vector2.zero; // Detén al jefe
        Debug.Log("Cambiando a la fase " + newPhase + ". Pausa de 4 segundos.");

        if (newPhase == 3)
        {
            while (!isGrounded)
            {
                yield return null; // Esperar hasta que el jefe esté en el suelo
            }
        }

        // Cambiar a la nueva fase
        bossPhase = newPhase;

        // Aquí puedes agregar cualquier lógica adicional que necesites al cambiar de fase
        yield return new WaitForSeconds(4f); // Espera de 4 segundos
        Debug.Log("Cambio de fase a " + newPhase + " completo.");
        canAttack = true; // Vuelve a habilitar los ataques
    }
}
