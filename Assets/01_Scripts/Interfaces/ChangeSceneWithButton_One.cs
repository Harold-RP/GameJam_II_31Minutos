using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneWithDoubleClick : MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource audioSource;

    public float doubleClickTime = 0.3f; // Tiempo m�ximo entre clics para considerar un doble clic
    private float lastClickTime = 0f; // Tiempo del �ltimo clic
    private int clickCount = 0; // Contador de clics

    [SerializeField] private float transitionTime = 1f; // Duraci�n de la animaci�n de transici�n
    private Animator transitionAnimator; // Referencia al Animator para la animaci�n de fade out

    private void Start()
    {
        // Obtener el AudioSource o agregarlo si no est� presente
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        // Obtener el Animator para la transici�n
        transitionAnimator = GetComponentInChildren<Animator>();
        if (transitionAnimator == null)
        {
            Debug.LogError("No se encontr� un Animator en los hijos del objeto.");
        }
    }

    private void Update()
    {
        // Detectar clic del rat�n
        if (Input.GetMouseButtonDown(0)) // Bot�n izquierdo del rat�n
        {
            clickCount++;

            // Verificar si es un doble clic
            if (clickCount == 1)
            {
                lastClickTime = Time.time;
            }
            else if (clickCount == 2 && (Time.time - lastClickTime) < doubleClickTime)
            {
                // Si es un doble clic, iniciar la transici�n de escena
                StartCoroutine(SceneLoad("StartMenu")); // Cambia "StartMenu" por el nombre real de tu escena
                clickCount = 0; // Reiniciar el contador de clics
            }
        }

        // Restablecer el contador de clics si ha pasado demasiado tiempo entre clics
        if ((Time.time - lastClickTime) > doubleClickTime)
        {
            clickCount = 0;
        }
    }

    private IEnumerator SceneLoad(string sceneName)
    {
        // Reproducir el sonido de clic si est� disponible
        if (clickSound != null)
        {
            audioSource.clip = clickSound;
            audioSource.Play();
        }

        // Ejecutar la animaci�n de transici�n si el Animator est� configurado
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("StartTransition");
        }

        // Esperar a que termine la animaci�n
        yield return new WaitForSeconds(transitionTime);

        // Cargar la nueva escena
        SceneManager.LoadScene(sceneName);
    }
}
