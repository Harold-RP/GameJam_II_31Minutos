using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player playerHealth = collision.gameObject.GetComponent<Player>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
        // Destruir la bala tras impactar
        Destroy(gameObject);
    }
}
