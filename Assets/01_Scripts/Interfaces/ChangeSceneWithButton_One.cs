using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneWithButton_One : MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource audioSource;

    public float doubleClickTime = 0.3f;
    private float lastClickTime = 0f; 
    private int clickCount = 0;

    [SerializeField] private float transitionTime = 1f; 
    private Animator transitionAnimator;

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

    private void Update()
    {
        
        if (Input.GetMouseButtonDown(0)) 
        {
            clickCount++;

           
            if (clickCount == 1)
            {
                lastClickTime = Time.time;
            }
            else if (clickCount == 2 && (Time.time - lastClickTime) < doubleClickTime)
            {
                
                StartCoroutine(SceneLoad("StartMenu")); 
                clickCount = 0;
            }
        }

     
        if ((Time.time - lastClickTime) > doubleClickTime)
        {
            clickCount = 0;
        }
    }

    private IEnumerator SceneLoad(string sceneName)
    {
      
        if (clickSound != null)
        {
            audioSource.clip = clickSound;
            audioSource.Play();
        }

       
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("StartTransition");
        }

        
        yield return new WaitForSeconds(transitionTime);

       
        SceneManager.LoadScene(sceneName);
    }
}
