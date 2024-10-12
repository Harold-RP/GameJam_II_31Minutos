using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("---------------------- Stats ------------------------")]
    public float timeBtwSpawn = 15f;
    public float timer = 0f;
    [Header("---------------------- References ------------------------")]
    public Transform leftPoint;
    public Transform rightPoint;
    public List<PowerUp> powerUpList = new List<PowerUp>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < timeBtwSpawn)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            int i = Random.Range(0, powerUpList.Count);
            float posX = Random.Range(leftPoint.position.x, rightPoint.position.x);
            Instantiate(powerUpList[i], new Vector3(posX, transform.position.y), transform.rotation);
        }
    }
}
