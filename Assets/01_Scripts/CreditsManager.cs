using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    public float creditsDuration = 50f;
    public TextMeshProUGUI gameTitle;
    public RectTransform allCredits;
    public RectTransform returnToMenuButton;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.PlayInstrumentalAndVocals("", "");
        AudioManager.instance.InstrumentalVolumeUp(1f);
        AudioManager.instance.VocalsVolumeUp(1f);
        LeanTween.value(gameObject, 0f, 255f, 2f)
                        .setOnUpdate((float value) => {
                            Color color = gameTitle.color;
                            color.a = value / 255f;
                            gameTitle.color = color;
                        }).setDelay(1f).setOnComplete(ShowCredits);
    }

    void ShowCredits()
    {
        LeanTween.moveY(allCredits, -1840, creditsDuration).setDelay(2f).setOnComplete(ShowReturnToMenu);
    }

    void ShowReturnToMenu()
    {        
        LeanTween.value(gameObject, 0f, 255f, 1f)
                        .setOnUpdate((float value) => {
                            Color color = returnToMenuButton.gameObject.GetComponent<Image>().color;
                            color.a = value / 255f;
                            returnToMenuButton.gameObject.GetComponent<Image>().color = color;
                        }).setDelay(3f);
        returnToMenuButton.gameObject.GetComponent<Button>().interactable = true;
    }
}
