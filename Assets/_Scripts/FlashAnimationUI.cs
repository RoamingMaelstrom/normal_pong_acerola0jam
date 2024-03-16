using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashAnimationUI : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float numFlashes;
    [SerializeField] float duration;
    [SerializeField] bool disableAtEnd;


    public void PlayFlashingAnimation(){
        StartCoroutine(StandardFlash(numFlashes, duration, disableAtEnd));
    }

    private IEnumerator StandardFlash(float numFlashes, float duration, bool disableAtEnd)
    {
        float flashDuration = duration * 0.5f / numFlashes;
        Color startColour = image.color;
        bool isTransparent = false;

        for (int i = 0; i < numFlashes * 2; i++)
        {
            isTransparent = !isTransparent;
            image.color = isTransparent ? new Color(0, 0, 0, 0): startColour;
            yield return new WaitForSeconds(flashDuration);
        }

        if (disableAtEnd) image.gameObject.SetActive(false);
    }
}
