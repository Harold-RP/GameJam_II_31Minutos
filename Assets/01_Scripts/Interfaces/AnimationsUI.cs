using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsUI : MonoBehaviour
{
    [SerializeField] private GameObject logo;         
    [SerializeField] private GameObject inicioGrupo; 

    private CanvasGroup logoCanvasGroup;
    private CanvasGroup inicioGrupoCanvasGroup;

    private void Start()
    {
     
        logoCanvasGroup = logo.GetComponent<CanvasGroup>();
        if (logoCanvasGroup == null)
        {
            logoCanvasGroup = logo.AddComponent<CanvasGroup>();
        }
        logoCanvasGroup.alpha = 0; 

      
        inicioGrupoCanvasGroup = inicioGrupo.GetComponent<CanvasGroup>();
        if (inicioGrupoCanvasGroup == null)
        {
            inicioGrupoCanvasGroup = inicioGrupo.AddComponent<CanvasGroup>();
        }
        inicioGrupoCanvasGroup.alpha = 1; 

     
        StartCoroutine(AnimateLogoAndGroup());
    }

    private IEnumerator AnimateLogoAndGroup()
    {

        yield return StartCoroutine(FadeCanvasGroup(logoCanvasGroup, 0f, 1f, 3f));

        yield return new WaitForSeconds(2f);


        yield return StartCoroutine(FadeCanvasGroup(logoCanvasGroup, 1f, 0f, 1f));

        yield return StartCoroutine(FadeCanvasGroup(inicioGrupoCanvasGroup, 1f, 0f, 1f));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }
}
