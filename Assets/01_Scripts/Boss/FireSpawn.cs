using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpawn : MonoBehaviour
{
    public GameObject firePrefab;     // Prefab del fuego que se instanciar�
    public float fireDuration = 3f;   // Duraci�n del fuego antes de desaparecer

    // M�todo para instanciar el fuego en la posici�n del FireSpawn
    public void ActivateFire()
    {
        StartCoroutine(SpawnFire());
    }

    // Corrutina para manejar la aparici�n y desaparici�n del fuego
    IEnumerator SpawnFire()
    {
        // Instanciar el prefab de fuego en la posici�n del FireSpawn
        GameObject fireInstance = Instantiate(firePrefab, transform.position, transform.rotation);

        // El fuego permanece en la escena por el tiempo definido en fireDuration
        yield return new WaitForSeconds(fireDuration);

        // Destruir la instancia del fuego despu�s de la duraci�n
        Destroy(fireInstance);
    }
}
