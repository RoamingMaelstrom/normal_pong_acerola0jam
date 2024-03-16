using System.Collections;
using UnityEngine;
using TMPro;

public class FlashAnimationText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float numFlashes;
    [SerializeField] float duration;
    [SerializeField] bool disableAtEnd;


    public void PlayFlashingAnimation(){
        StartCoroutine(StandardFlash(numFlashes, duration, disableAtEnd));
    }

    private IEnumerator StandardFlash(float numFlashes, float duration, bool disableAtEnd)
    {
        float flashDuration = duration * 0.5f / numFlashes;
        Color startColour = text.color;
        bool isTransparent = false;

        for (int i = 0; i < numFlashes * 2; i++)
        {
            isTransparent = !isTransparent;
            text.color = isTransparent ? new Color(0, 0, 0, 0): startColour;
            yield return new WaitForSecondsRealtime(flashDuration);
        }

        if (disableAtEnd) text.gameObject.SetActive(false);
    }
}
