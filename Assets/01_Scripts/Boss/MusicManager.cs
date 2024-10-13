using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;        // Fuente de audio que reproducirá la música
    public AudioClip introMusic;           // Música de introducción
    public AudioClip[] phaseMusicClips;    // Lista de música para cada fase
    public Boss boss;                      // Referencia al script del jefe

    private int currentPhase = -1;         // Variable para almacenar la fase actual

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (boss == null)
        {
            Debug.LogError("No se ha asignado un script de jefe al Music Manager.");
            return;
        }

        PlayMusic(introMusic);  // Reproducir la música de introducción al inicio
    }

    void Update()
    {
        // Verifica si la fase del jefe ha cambiado
        if (boss.bossPhase != currentPhase)
        {
            currentPhase = boss.bossPhase;
            ChangeMusicForPhase(currentPhase);
        }
    }

    private void PlayMusic(AudioClip musicClip)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();  // Detener la música actual
        }

        // Asignar la nueva música y reproducirla
        audioSource.clip = musicClip;
        audioSource.Play();
    }

    private void ChangeMusicForPhase(int phase)
    {
        if (phase >= 1 && phase <= phaseMusicClips.Length)
        {
            PlayMusic(phaseMusicClips[phase - 1]);
            Debug.Log("Música cambiada para la fase " + phase);
        }
    }
}
