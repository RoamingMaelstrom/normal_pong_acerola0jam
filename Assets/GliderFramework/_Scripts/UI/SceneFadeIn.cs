using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFadeIn : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float delayBeforeFadeIn = 1f;
    [SerializeField] bool startScreenCovered = true;

    private void Awake() 
    {
        if (startScreenCovered)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            return;
        }

        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }



    private void Start() 
    {
        FadeIn();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeIn(fadeDuration));
    }

    private IEnumerator FadeIn(float fadeDuration)
    {
        yield return new WaitForSecondsRealtime(delayBeforeFadeIn);

        float rateOfFade = Time.fixedDeltaTime / fadeDuration;

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= rateOfFade;
            yield return new WaitForFixedUpdate();
        }

        canvasGroup.blocksRaycasts = false;
    }
}
