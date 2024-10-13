using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private float moveDistance;
    private float moveSpeed;
    private Vector3 startPosition;

    // Inicializar los valores de movimiento desde la mano
    public void InitMovement(float distance, float speed, Vector3 startPos)
    {
        moveDistance = distance;
        moveSpeed = speed;
        startPosition = startPos;
        StartCoroutine(MoveUpDown());
    }

    IEnumerator MoveUpDown()
    {
        while (true)
        {
            float newY = startPosition.y + Mathf.PingPong(Time.time * moveSpeed, moveDistance);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            yield return null;
        }
    }
}
