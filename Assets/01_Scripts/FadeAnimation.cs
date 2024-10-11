using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeAnimation : MonoBehaviour
{
    public Animator animator;

    public void LoadScene(string name)
    {
        StartCoroutine(Transition(name));
    }

    IEnumerator Transition(string scene)
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1f);
        LevelLoader.LoadLevel(scene);
    }



    public void LoadSceneWithoutLoading(string name)
    {
        StartCoroutine(TransitionNoLoadingScreen(name));
    }

    IEnumerator TransitionNoLoadingScreen(string scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                // La escena está cargada, ahora inicia el fade out
                animator.SetTrigger("FadeOut");
                yield return new WaitForSeconds(1f);//espera 1 seg para el fadeOut
                operation.allowSceneActivation = true;//Activa el cambio de escena
            }
            yield return null;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
