using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCoverBullet : MonoBehaviour
{
    public float speed = 3f;
    public float lifeTime = 5f;
    public float rotationSpeed = 50f;
    Transform player;


    public AudioSource audioSource;   // Referencia al AudioSource
    public AudioClip creep;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float angleToPlayer = Vector3.SignedAngle(transform.up, directionToPlayer, Vector3.forward);
            float rotationAmount = Mathf.Clamp(angleToPlayer, -rotationSpeed * Time.deltaTime, rotationSpeed * Time.deltaTime);
            transform.Rotate(0, 0, rotationAmount);
        }

        transform.Translate(Vector2.up * speed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(creep);
            ScreenCoverUI ui = collision.gameObject.GetComponent<ScreenCoverUI>();
            ui.CoverScreen();
            Destroy(gameObject);
        }
    }
}
