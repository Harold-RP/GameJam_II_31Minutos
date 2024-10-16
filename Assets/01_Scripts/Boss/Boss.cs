using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public float contactDamage = 1f;

    public float laserDuration = 0.5f;
    public int bossPhase = 1;
    bool isAttacking = false;

    public float maxHealth = 200f; 
    public float currentHealth;
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
    public TextMeshProUGUI lifeText;
    public Image lifeBar;

    public Animator animator;
    public Animator fadeAnim;

    public List<GameObject> wallpaperObjects;

    public AudioSource audioSource;   
    public AudioClip charge;
    public AudioClip jump;
    public AudioClip laser;
    public AudioClip teleport;
    public AudioClip fire;
    public AudioClip proyectiles;
    public AudioClip dam;
    public AudioClip death;
    public AudioClip walk;
    public AudioClip final;

    void Start()
    {

        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        UpdateLifeBar();

        UpdateWallpaper();

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
                    animator.SetInteger("phase", 2);
                    TeleportToRandomCorner();
                    Mirror();
                    AimAtPlayer();
                    StartCoroutine(LaserAttack());
                    break;
                case 3:
                    animator.SetInteger("phase", 3);
                    SpawnHead();
                    break;
                case 4:
                    animator.SetInteger("phase", 3);
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
                    animator.SetInteger("phase", 5);
                    Mirror();
                    AimAtPlayer();
                    StartCoroutine(Phase5Attack());
                    break;
            }

            // Actualiza el wallpaper en función de la fase actual del jefe
            UpdateWallpaper();
        }
    }

    void UpdateWallpaper()
    {
        // Asegurarse de que haya suficientes objetos en la lista
        if (bossPhase >= 1 && bossPhase <= wallpaperObjects.Count)
        {
            // Desactivar todos los wallpapers
            foreach (GameObject wallpaper in wallpaperObjects)
            {
                wallpaper.SetActive(false);
            }

            // Activar solo el wallpaper correspondiente a la fase actual
            wallpaperObjects[bossPhase - 1].SetActive(true);
            Debug.Log("Cambiado a wallpaper del objeto de la fase " + bossPhase);
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
        audioSource.PlayOneShot(charge);
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

            audioSource.PlayOneShot(jump);
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

        audioSource.PlayOneShot(laser);
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
        audioSource.PlayOneShot(teleport);
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateLifeBar();
        audioSource.PlayOneShot(dam);
        if (currentHealth <= 0 && bossPhase > 5)
        {
            StartCoroutine(Die());
        }
        else
        {
            CheckPhaseChange();
        }
    }

    IEnumerator Die()
    {
        audioSource.PlayOneShot(death);
        Debug.Log("El jefe ha sido derrotado.");
        AsyncOperation operation = SceneManager.LoadSceneAsync("Scene2");
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
        GameObject.Destroy(gameObject);
    }

    void UpdateLifeBar()
    {
        lifeBar.fillAmount = currentHealth / maxHealth;
        lifeText.text = "" + (int)currentHealth;
        if (lifeBar.fillAmount < 1f / 3f)
        {
            lifeBar.color = Color.red;
            lifeText.color = Color.red;
        }
        else if (lifeBar.fillAmount < 2f / 3f)
        {
            lifeBar.color = Color.yellow;
            lifeText.color = Color.yellow;
        }
        else
        {
            lifeBar.color = Color.green;
            lifeText.color = Color.green;
        }
    }


    //Phase 3 ------------------------------------------------------------------------
    void SpawnHead()
    {
        if (!isHeadSpawned) // Si la cabeza no ha sido generada aún
        {
            isHeadSpawned = true;
            collider.enabled = false; // Desactivar collider del jefe
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY; // Congelar completamente el jefe
            bossHeadGO = Instantiate(headPrefab, new Vector3(0, 0), transform.rotation);
        }
        else // Cabeza ya generada, revisar si sigue viva
        {
            if (bossHeadGO == null) // La cabeza ha sido destruida
            {
                isHeadSpawned = false;
                rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY; // Liberar restricción en Y
                rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX; // Liberar restricción en X
                collider.enabled = true; // Reactivar collider
                bossPhase = 4; // Pasar a la siguiente fase
            }
        }
    }

    public void OnBossHeadDestroyed()
    {
        Destroy(bossHeadGO);
        isHeadSpawned = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        collider.enabled = true; // Reactivar collider del jefe
        bossPhase = 4; // Cambiar a la fase 4
        Debug.Log("La cabeza del jefe ha sido destruida. Cambiando a la fase 4.");
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
        audioSource.PlayOneShot(fire);
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
        audioSource.PlayOneShot(walk);
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * Phase4Speed, rb.velocity.y);  // Ajusta `moveSpeed` a la velocidad que desees
        Debug.Log("El jefe se mueve hacia el jugador.");
    }

    //Phase 5 ------------------------------------------------------------------------
    IEnumerator Phase5Attack()
    {
        lifeBar.enabled = false;
        lifeText.enabled = false;
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

        audioSource.PlayOneShot(final);

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

        if (collision.gameObject.CompareTag("Player"))
        {
            // Intentar obtener el script de vida o daño del jugador
            Player playerHealth = collision.gameObject.GetComponent<Player>();

            if (playerHealth != null)
            {
                // Aplicar daño al jugador
                playerHealth.TakeDamage(contactDamage);
            }
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

    //void CheckPhaseChange()
    //{
    //    // Cada 100 de vida cambiamos de fase
    //    if (currentHealth <= 400 && bossPhase == 1)
    //    {
    //        StartCoroutine(ChangePhase(2));
    //    }
    //    else if (currentHealth <= 300 && bossPhase == 2)
    //    {
    //        StartCoroutine(ChangePhase(3));
    //        rb.isKinematic = false;
    //    }
    //    else if (currentHealth <= 200 && bossPhase == 3)
    //    {
    //        rb.freezeRotation = true;
    //        StartCoroutine(ChangePhase(4));
    //    }
    //    else if (currentHealth <= 100 && bossPhase == 4)
    //    {
    //        StartCoroutine(ChangePhase(5));
    //    }
    //}

    void CheckPhaseChange()
    {
        if (currentHealth <= 0)
        {
            if (bossPhase == 2)
            {
                rb.isKinematic = false;
            }
            else if (bossPhase == 3)
            {
                rb.freezeRotation = true;
            }
            bossPhase++;
            if (bossPhase > 5)
            {
                return;
            }
            StartCoroutine(ChangePhase(bossPhase));
            currentHealth = maxHealth;
            UpdateLifeBar();
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
    public void UpdateSharedLife(float damage)
    {
        currentHealth += damage; // Actualizar la vida total
        UpdateLifeBar(); // Actualizar la barra de vida
    }
}
