using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("---------------------- Stats ------------------------")]
    public float timeBtwSpawn = 15f;
    public float timer = 0f;
    public int enemiesToSpawn = 5;
    [Header("---------------------- References ------------------------")]
    public GameObject enemyPrefab;
    public Transform leftPoint;
    public Transform rightPoint;

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
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            if (Random.Range(0, 2) == 0)
            {
                Instantiate(enemyPrefab, leftPoint.position, leftPoint.rotation);
            }
            else
            {
                Instantiate(enemyPrefab, rightPoint.position, rightPoint.rotation);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
}
