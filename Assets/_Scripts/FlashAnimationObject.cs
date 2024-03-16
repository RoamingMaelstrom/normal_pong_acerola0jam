using System.Collections;
using UnityEngine;

public class FlashAnimationObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float numFlashes;
    [SerializeField] float duration;
    [SerializeField] bool disableAtEnd;

    public void PlayFlashingAnimation(){
        StartCoroutine(StandardFlash(numFlashes, duration, disableAtEnd));
    }

    private IEnumerator StandardFlash(float numFlashes, float duration, bool disableAtEnd)
    {
        float flashDuration = duration * 0.5f / numFlashes;
        Color startColour = spriteRenderer.color;
        bool isTransparent = false;

        for (int i = 0; i < numFlashes * 2; i++)
        {
            isTransparent = !isTransparent;
            spriteRenderer.color = isTransparent ? new Color(0, 0, 0, 0): startColour;
            yield return new WaitForSeconds(flashDuration);
        }

        if (disableAtEnd) spriteRenderer.gameObject.SetActive(false);
    }
}
