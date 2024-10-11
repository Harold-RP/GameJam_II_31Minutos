using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        string LevelToLoad = LevelLoader.nextLevel;
        StartCoroutine(MakeTheLoad(LevelToLoad));
    }

    IEnumerator MakeTheLoad(string level)
    {
        yield return new WaitForSeconds(3f); //simula el tiempo de carga
        AsyncOperation operation = SceneManager.LoadSceneAsync(level);
        operation.allowSceneActivation = false;
        switch (level)
        {
            case "Game":
                string instClipName = "";
                string vocalsClipName = "";
                if (AudioManager.instance.instrumentalAS.clip.name != instClipName)
                {
                    AudioManager.instance.PlayInstrumentalAndVocals(instClipName, vocalsClipName);
                }
                break;
        }
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                //----------Para activar las voces del BGM---------
                AudioManager.instance.VocalsVolumeUp(5f);
                // La escena está cargada, ahora inicia el fade out
                animator.SetTrigger("FadeOut");
                yield return new WaitForSeconds(1f);//espera 1 seg para el fadeOut
                operation.allowSceneActivation = true;//Activa el cambio de escena
            }
            yield return null;
        }
    }
}
