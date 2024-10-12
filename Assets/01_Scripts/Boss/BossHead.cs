using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossHead : MonoBehaviour
{
    public GameObject bulletPrefab;
    public UnityEvent<GameObject> onShoot;
    public float timer = 0f;
    public float timeBtwShoot = 5f;
    public float speed = 5f;
    public Transform firePoints;
    public Transform leftLimit;
    public Transform rightLimit;
    public Transform downLimit;
    public float upLimit = 7f;
    bool isGoingLeft = false;
    bool isMovingVertical = false;
    bool isGoingDown = true;
    public float TimeBtwDirection = 3f;
    float directionTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        leftLimit = GameObject.FindWithTag("LeftSpawner").transform;
        rightLimit = GameObject.FindWithTag("RightSpawner").transform;
        downLimit = GameObject.FindWithTag("Spawner").transform;
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
            if (downLimit.position.y + 1 < transform.position.y)
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
}
