using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class TrailerManager : MonoBehaviour
{
    public Animator fadeAnimator;
    VideoPlayer video;
    RawImage rawImg;
    float timer = 0f;
    public float inactivityTimer = 30f;
    bool isTransitioning = false;

    void Awake()
    {
        video = GetComponent<VideoPlayer>();
        rawImg = GetComponent<RawImage>();
        video.enabled = false;
        rawImg.enabled = false;
        video.loopPointReached += CheckOver;
    }

    private void Update()
    {
        if (Input.anyKey && !isTransitioning)
        {
            if (video.isPlaying)
                StartCoroutine(StartStopTrailer());
            else
                timer = 0f;
        }
        else//no hay entrada
        {
            if (timer < inactivityTimer && !video.isPlaying && !isTransitioning)
                timer += Time.deltaTime;
            else if(!video.isPlaying && !isTransitioning)//activar video
                StartCoroutine(StartStopTrailer());
        }
    }

    IEnumerator StartStopTrailer()
    {
        isTransitioning = true;
        timer = 0f;
        fadeAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlayPauseBGM();
        video.enabled = !video.enabled;
        rawImg.enabled = !rawImg.enabled;
        fadeAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1f);
        isTransitioning = false;
    }

    void CheckOver(VideoPlayer vp)//se ejecuta cuando el video termina
    {
        if (!isTransitioning)
        {
            StartCoroutine(StartStopTrailer());
        }
    }
}
