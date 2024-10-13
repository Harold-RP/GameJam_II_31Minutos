using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSound : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip; 
    [SerializeField] private float delayBeforePlaying = 0f; 
    [SerializeField] private Button button1; 
    [SerializeField] private Button button2; 
    [SerializeField] private Button button3; 

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

       
        audioSource.clip = musicClip;
        audioSource.playOnAwake = false;

      
        StartCoroutine(PlayMusicWithDelay());

       
        if (button1 != null) button1.onClick.AddListener(StopMusic);
        if (button2 != null) button2.onClick.AddListener(StopMusic);
        if (button3 != null) button3.onClick.AddListener(StopMusic);
    }

    private IEnumerator PlayMusicWithDelay()
    {
        
        yield return new WaitForSeconds(delayBeforePlaying);
        audioSource.Play();
    }

 
    public void RestartMusic()
    {
        audioSource.Stop();
        audioSource.Play();
    }

   
    public void StopMusic()
    {
        audioSource.Stop();
    }

  
    public void SetDelay(float newDelay)
    {
        delayBeforePlaying = newDelay;
    }

   
    public void PlayMusic()
    {
        audioSource.Play();
    }
}
