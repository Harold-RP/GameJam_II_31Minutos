using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCoverUI : MonoBehaviour
{
    public RectTransform coverImg;
    public float timePerCover = 3f;
    float timer = 0f;
    bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                LeanTween.moveY(coverImg, 0, 1f);
            }
            else
            {
                isActive = false;
            }
        }
        else
        {
            LeanTween.moveY(coverImg, -1100, 1f);
        }
    }

    public void CoverScreen()
    {
        timer += timePerCover;
        isActive = true;
    }
}
