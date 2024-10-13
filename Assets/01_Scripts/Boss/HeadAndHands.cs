using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadAndHands : MonoBehaviour
{

    private float sharedLife;
    public GameObject bigBulletPrefab; // Prefab de la bala grande
    public Transform firePoint;
    public float attackCooldown = 2f; // Tiempo de espera entre ataques
    private bool isAttacking = false;

    public float moveDistance = 0.5f; // Distancia máxima de movimiento
    public float moveSpeed = 1f; // Velocidad del movimiento
    private Vector3 startPosition;

    public GameObject platformPrefab; // Prefab de la plataforma
    private bool isHandDestroyed = false; // Controla si la mano ya ha sido destruida   

    private static int totalHands = 2; // Total de manos en la escena
    private static int destroyedHands = 0; // Contador de manos destruidas

    private bool isHeadActive = false;
    public float increasedMoveSpeed = 8f;

    private Transform player;
    private Boss bossController;
    public void SetSharedLife(float life)
    {
        sharedLife = life;
        startPosition = transform.position; // Guardar la posición inicial
        StartCoroutine(MoveUpDown());
    }

    void Update()
    {
        // Controlar vida compartida y destruir partes
        if (sharedLife <= 0)
        {
            if (this.CompareTag("Hand") && !isHandDestroyed)
            {
                // Convertir la mano en una plataforma cuando se destruya
                InstantiatePlatform();
                isHandDestroyed = true;
                destroyedHands++; // Incrementar el contador de manos destruidas
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Lógica de ataque solo para las manos que no han sido destruidas
        if (this.CompareTag("Hand") && !isAttacking && !isHandDestroyed)
        {
            StartCoroutine(Phase5Attack());
        }

        if (destroyedHands >= totalHands && !isHeadActive)
        {
            isHeadActive = true; // Activar la lógica de la cabeza
            moveSpeed = increasedMoveSpeed; // Aumentar la velocidad de movimiento
            StartCoroutine(HeadAttackRoutine()); // Comenzar a atacar
        }

    }

    public void SetBossController(Boss controller)
    {
        bossController = controller;
    }

    IEnumerator MoveUpDown()
    {
        while (true) // Loop infinito para el movimiento
        {
            float newY = startPosition.y + Mathf.PingPong(Time.time * moveSpeed, moveDistance); // Calcular la nueva posición en Y
            transform.position = new Vector3(transform.position.x, newY, transform.position.z); // Aplicar la nueva posición

            yield return null; // Esperar un frame
        }
    }

    IEnumerator Phase5Attack()
    {
        if (bigBulletPrefab == null || firePoint == null)
        {
            Debug.LogError("bigBulletPrefab or firePoint is not assigned!");
            yield break; // Salir del método si hay un error
        }

        Debug.Log("La mano lanza 3 balas grandes que rebotarán.");



        isAttacking = true;

        // Lanzar 3 balas grandes
        for (int i = 0; i < 3; i++)
        {
            Instantiate(bigBulletPrefab, firePoint.position, firePoint.rotation);

            yield return new WaitForSeconds(0.5f); // Intervalo entre disparos
        }

        // Esperar antes de que la mano pueda atacar de nuevo
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }

    IEnumerator HeadAttackRoutine()
    {
        while (isHeadActive) // Ataque mientras la cabeza está activa
        {
            yield return StartCoroutine(Phase5Attack()); // Llamar a la función de ataque
            yield return new WaitForSeconds(attackCooldown); // Esperar antes de atacar de nuevo
        }
    }

    void InstantiatePlatform()
    {
        if (platformPrefab != null)
        {
            GameObject platform = Instantiate(platformPrefab, transform.position, Quaternion.identity);
            platform.GetComponent<Platform>().InitMovement(moveDistance, moveSpeed, startPosition); // Pasar los valores de movimiento a la plataforma
        }
        Destroy(gameObject); // Destruir la mano original
    }


    public void TakeDamage(float damage)
    {
        if (this.CompareTag("Head"))
        {
            if (destroyedHands == totalHands)
            {

                sharedLife -= damage;
               

                // Verificar si la cabeza es destruida
                if (sharedLife <= 0)
                {
                    Destroy(gameObject); // Destruir la cabeza
                }
            }
            else
            {
                Debug.Log("No se puede dañar la cabeza hasta que todas las manos sean destruidas.");
            }
        }
         else if (this.CompareTag("Hand"))
        {
            sharedLife -= damage; // Aplicar daño a las manos
           // bossController.UpdateSharedLife(-damage);
        }

    }

   

}
