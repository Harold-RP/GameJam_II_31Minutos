using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsSound : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip; 
    [SerializeField] private float delayBeforePlaying = 0f; 
    [SerializeField] private Button stopButton;

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

        
        if (stopButton != null) stopButton.onClick.AddListener(StopMusic);
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
