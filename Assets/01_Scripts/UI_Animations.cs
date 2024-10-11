using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Animations : MonoBehaviour
{
    public GameObject intro;

    void Start()
    {
        LeanTween.alpha(intro.GetComponent<RectTransform>(), 0, 1f).setDelay(3f).setOnComplete(InitMainMenu);
    }

    void InitMainMenu()
    {
        intro.GetComponentInChildren<Image>().raycastTarget = false;
    }
}
