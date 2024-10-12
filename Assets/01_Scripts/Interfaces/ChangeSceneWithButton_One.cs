using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneWithDoubleClick : MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource audioSource;

    public float doubleClickTime = 0.3f; // Tiempo máximo entre clics para considerar un doble clic
    private float lastClickTime = 0f; // Tiempo del último clic
    private int clickCount = 0; // Contador de clics

    [SerializeField] private float transitionTime = 1f; // Duración de la animación de transición
    private Animator transitionAnimator; // Referencia al Animator para la animación de fade out

    private void Start()
    {
        // Obtener el AudioSource o agregarlo si no está presente
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        // Obtener el Animator para la transición
        transitionAnimator = GetComponentInChildren<Animator>();
        if (transitionAnimator == null)
        {
            Debug.LogError("No se encontró un Animator en los hijos del objeto.");
        }
    }

    private void Update()
    {
        // Detectar clic del ratón
        if (Input.GetMouseButtonDown(0)) // Botón izquierdo del ratón
        {
            clickCount++;

            // Verificar si es un doble clic
            if (clickCount == 1)
            {
                lastClickTime = Time.time;
            }
            else if (clickCount == 2 && (Time.time - lastClickTime) < doubleClickTime)
            {
                // Si es un doble clic, iniciar la transición de escena
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
        // Reproducir el sonido de clic si está disponible
        if (clickSound != null)
        {
            audioSource.clip = clickSound;
            audioSource.Play();
        }

        // Ejecutar la animación de transición si el Animator está configurado
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("StartTransition");
        }

        // Esperar a que termine la animación
        yield return new WaitForSeconds(transitionTime);

        // Cargar la nueva escena
        SceneManager.LoadScene(sceneName);
    }
}
