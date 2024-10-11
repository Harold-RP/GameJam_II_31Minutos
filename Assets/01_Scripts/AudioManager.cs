using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("--------------- Variables ---------------")]
    public float instrumentalVol = 0.5f;
    public float vocalsVol = 0f;
    public float sfxVol = 0.7f;
    [Header("--------------- Audio Sources ---------------")]
    public AudioSource instrumentalAS;
    public AudioSource vocalsAS;
    public AudioSource sfxAS;
    [Header("--------------- Audio Clips ---------------")]
    public List<AudioClip> BGM_Instrumental = new List<AudioClip>();
    public List<AudioClip> BGM_Vocals = new List<AudioClip>();
    public List<AudioClip> SFX = new List<AudioClip>();

    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        instrumentalAS.volume = instrumentalVol;
        instrumentalAS.loop = true;
        instrumentalAS.playOnAwake = true;

        vocalsAS.volume = vocalsVol;
        vocalsAS.loop = true;
        vocalsAS.playOnAwake = true;

        sfxAS.volume = sfxVol;
        sfxAS.loop = false;
        sfxAS.playOnAwake = false;
    }

    void Start()
    {
        StartCoroutine(InitialMusic());
    }

    IEnumerator InitialMusic()
    {
        yield return new WaitForSeconds(3f);
        PlayInstrumentalAndVocals("", "");
    }

    public void PlaySFX(AudioClip sound)
    {
        sfxAS.PlayOneShot(sound);
    }

    public void PlayInstrumentalAndVocals(string instrumental, string vocals)
    {
        vocalsAS.Stop();
        instrumentalAS.Stop();
        instrumentalAS.clip = BGM_Instrumental.Find(i => i.name == instrumental);        
        instrumentalAS.Play();
        if (vocals != null)
        {
            vocalsAS.clip = BGM_Vocals.Find(v => v.name == vocals);
            vocalsAS.Play();
        }
    }

    public void PlayPauseBGM()
    {
        if (instrumentalAS.isPlaying)
        {
            instrumentalAS.Pause();
            vocalsAS.Pause();
        }
        else
        {
            instrumentalAS.Play();
            vocalsAS.Play();
        }
    }

    public void VocalsVolumeUp(float duration)
    {
        GameObject aS = vocalsAS.gameObject;
        float initialValue = vocalsAS.volume;
        float finalValue = instrumentalAS.volume * 0.6f;
        LeanTween.value(aS, initialValue, finalValue, duration)
                 .setOnUpdate((float value) => {
                     vocalsAS.volume = value;
                 });
    }

    public void VocalsVolumeDown(float duration)
    {
        GameObject aS = vocalsAS.gameObject;
        float initialValue = vocalsAS.volume;
        float finalValue = 0f;
        LeanTween.value(aS, initialValue, finalValue, duration)
                 .setOnUpdate((float value) => {
                     vocalsAS.volume = value;
                 });
    }

    public void InstrumentalVolumeUp(float duration)
    {
        GameObject aS = instrumentalAS.gameObject;
        float initialValue = 0f;
        float finalValue = instrumentalVol;
        LeanTween.value(aS, initialValue, finalValue, duration)
                 .setOnUpdate((float value) => {
                     instrumentalAS.volume = value;
                 });
    }

    public void InstrumentalVolumeDown(float duration)
    {
        GameObject aS = instrumentalAS.gameObject;
        float initialValue = instrumentalVol;
        float finalValue = 0f;
        LeanTween.value(aS, initialValue, finalValue, duration)
                 .setOnUpdate((float value) => {
                     instrumentalAS.volume = value;
                 });
    }
}
