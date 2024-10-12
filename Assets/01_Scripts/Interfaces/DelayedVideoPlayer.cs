using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class DelayedVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public float delayInSeconds = 5f;

    private void Start()
    {
       
        StartCoroutine(PlayVideoAfterDelay());
    }

    private IEnumerator PlayVideoAfterDelay()
    {
        
        yield return new WaitForSeconds(delayInSeconds);
        videoPlayer.Play();
    }
}
