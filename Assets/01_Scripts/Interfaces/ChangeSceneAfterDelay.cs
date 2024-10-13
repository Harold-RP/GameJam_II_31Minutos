using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneAfterDelay : MonoBehaviour
{
    [SerializeField] private float delayTime = 7f; 

    private void Start()
    {
        StartCoroutine(ChangeSceneAfterDelayCoroutine());
    }

    private IEnumerator ChangeSceneAfterDelayCoroutine()
    {
       
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene("Game");
    }
}
