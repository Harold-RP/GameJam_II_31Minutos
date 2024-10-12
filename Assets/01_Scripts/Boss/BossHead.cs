using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    bool isGoingLeft = false;
    bool isMovingVertical = false;
    bool isGoingDown = true;
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
            boss = GetComponentInParent<Boss>(); // Si BossHead es un hijo del jefe
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
            if (leftLimit.position.x < transform.position.x)
                transform.Translate(Vector2.left * speed * Time.deltaTime);
            else
                isGoingLeft = false;
        }
        else//va a la derecha
        {
            if (transform.position.x < rightLimit.position.x)
                transform.Translate(Vector2.right * speed * Time.deltaTime);
            else
                isGoingLeft = true;
        }
    }

    public void TakeDamage(float damage)
    {
        life -= damage;
        Debug.Log("La cabeza del jefe ha recibido daño. Vida restante: " + life);
        if (life <= 0)
        {
            
            Destroy(gameObject);
           // boss.TakeDamage(100);
        }
    }
}
