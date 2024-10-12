using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsUI : MonoBehaviour
{
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject inicioGrupo;

    private void Start()
    {
        LeanTween.moveX(logo.GetComponent<RectTransform>(), 0, 1.5f).setDelay(2.5f)
        .setEase(LeanTweenType.easeOutBounce).setOnComplete(BajarAlpha);
    }

    private void BajarAlpha()
    {
        LeanTween.alpha(inicioGrupo.GetComponent<RectTransform>(), 0f, 1f).setDelay(3.5f);
        inicioGrupo.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

   
}
