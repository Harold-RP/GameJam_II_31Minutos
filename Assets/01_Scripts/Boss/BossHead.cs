using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BossHead : MonoBehaviour
{
    [Header("---------------------- Stats ------------------------")]
    public float life = 50f;
    public float timer = 0f;
    public float timeBtwShoot = 5f;
    public float speed = 5f;
    public float upLimit = 7f;
    public float TimeBtwDirection = 3f;
    float directionTimer = 0f;
    public bool isGoingLeft = true;
    bool isMovingVertical = true;
    bool isGoingDown = false;
    [Header("---------------------- References ------------------------")]
    public GameObject bulletPrefab;
    public Transform firePoints;
    public Transform leftLimit;
    public Transform rightLimit;
    public Transform downLimit;
    public UnityEvent<GameObject> onShoot;
    private Boss boss;

    // Start is called before the first frame update
    void Start()
    {
        leftLimit = GameObject.FindWithTag("LeftSpawner").transform;
        rightLimit = GameObject.FindWithTag("RightSpawner").transform;
        downLimit = GameObject.FindWithTag("Spawner").transform;

        if (boss == null)
        {
            //boss = GetComponentInParent<Boss>(); // Si BossHead es un hijo del jefe
            boss = GameObject.FindWithTag("Boss").GetComponent<Boss>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < timeBtwShoot)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            onShoot?.Invoke(bulletPrefab);
        }

        if (directionTimer < TimeBtwDirection && !isMovingVertical)
        {
            directionTimer += Time.deltaTime;
        }
        else
        {
            directionTimer = 0f;
            isMovingVertical = true;
        }

        if (isMovingVertical)
        {
            YMovement();
        }
        else
        {
            XMovement();
        }
    }

    void YMovement()
    {
        if (isGoingDown)// Bajar
        {
            if (downLimit.position.y < transform.position.y)
                transform.Translate(Vector2.down * speed * Time.deltaTime);
            else
                isGoingDown = false;
        }
        else//subir
        {
            if (upLimit > transform.position.y)
                transform.Translate(Vector2.up * speed * Time.deltaTime);
            else
            {
                isGoingDown = true;
                isMovingVertical = false;
            }
        }
    }

    void XMovement()
    {
        if (isGoingLeft)
        {
            if (leftLimit.position.x <= transform.position.x)
                transform.Translate(Vector3.left * speed * Time.deltaTime);
            else
                isGoingLeft = false;
        }
        else//va a la derecha
        {
            if (transform.position.x <= rightLimit.position.x)
                transform.Translate(Vector3.right * speed * Time.deltaTime);
            else
                isGoingLeft = true;
        }
    }



    public void TakeDamage(float damage)
    {
        if (boss.currentHealth-damage <= 0)
        {
            // Reactivar el jefe en la fase 4
            boss.OnBossHeadDestroyed();
            // Destruir la cabeza cuando la vida del jefe llegue a 0
            Destroy(gameObject);
        }
        else
        {
            boss.TakeDamage(damage); // Pasar el daño al jefe principal
        }
    }

}
