using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneWithButton_Two : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;          
    [SerializeField] private float transitionTime = 1f;    
    private Animator transitionAnimator;             
    private AudioSource audioSource;                       

    private void Start()
    {
      
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;


        transitionAnimator = GetComponentInChildren<Animator>();
        if (transitionAnimator == null)
        {
            Debug.LogError("No se encontró un Animator en los hijos del objeto.");
        }
    }

    public void OnClick(string sceneName)
    {
        StartCoroutine(SceneLoad(sceneName));
    }

    private IEnumerator SceneLoad(string sceneName)
    {
     
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

 
        yield return new WaitForSeconds(0.2f);


        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("StartTransition");
        }


        yield return new WaitForSeconds(transitionTime);


        SceneManager.LoadScene(sceneName);
    }

}
