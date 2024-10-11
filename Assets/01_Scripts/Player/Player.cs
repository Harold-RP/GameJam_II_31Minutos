using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;    // Velocidad de movimiento
    public float jumpForce = 10f;   // Fuerza del salto

    private Rigidbody2D rb;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Jump();
    }

    // Método para mover al jugador
    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal"); // Recibe el input de las teclas A/D o flechas
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y); // Movimiento horizontal
    }

    // Método para permitir saltar al jugador
    void Jump()
    {
        // Saltar cuando se presiona la tecla "Jump" (por defecto es la barra espaciadora) y está en el suelo
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Aplicar la fuerza del salto
        }
    }

    // Detectar cuando el jugador está en contacto con el suelo (usando el tag "floor")
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }

    // Detectar cuando el jugador deja de estar en contacto con el suelo
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }
}
