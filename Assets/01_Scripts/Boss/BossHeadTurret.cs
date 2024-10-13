using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BossHeadTurret : MonoBehaviour
{
   
   
    void OnEnable()
    {
        GetComponentInParent<BossHead>()?.onShoot.AddListener(Shoot);
    }

    void OnDisable()
    {
        GetComponentInParent<BossHead>()?.onShoot.RemoveListener(Shoot);
    }

    void Shoot(GameObject bulletPrefab)
    {
       
        Instantiate(bulletPrefab, transform.position, transform.rotation);
    }
}
