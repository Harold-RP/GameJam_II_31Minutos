using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpawn : MonoBehaviour
{
    public GameObject firePrefab;     // Prefab del fuego que se instanciará
    public float fireDuration = 3f;   // Duración del fuego antes de desaparecer

    // Método para instanciar el fuego en la posición del FireSpawn
    public void ActivateFire()
    {
        StartCoroutine(SpawnFire());
    }

    // Corrutina para manejar la aparición y desaparición del fuego
    IEnumerator SpawnFire()
    {
        // Instanciar el prefab de fuego en la posición del FireSpawn
        GameObject fireInstance = Instantiate(firePrefab, transform.position, transform.rotation);

        // El fuego permanece en la escena por el tiempo definido en fireDuration
        yield return new WaitForSeconds(fireDuration);

        // Destruir la instancia del fuego después de la duración
        Destroy(fireInstance);
    }
}
