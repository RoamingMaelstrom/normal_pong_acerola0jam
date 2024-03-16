using System;
using System.Collections;
using SOEvents;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using GliderAudio;

public class GameSceneOnLoadLogic : MonoBehaviour
{
    [SerializeField] SOEvent startGameEvent;
    [SerializeField] PostProcessVolume ppVolume;
    [SerializeField] float startCAberration = 0.5f;
    [SerializeField] float endCAberration = 0.02f;
    [SerializeField] float transitionDuration = 3f;
    [SerializeField] int mainGameTrackContainerIndex;
    [SerializeField] string enterSceneSfx;

    private void Start() {
        StartCoroutine(ChromeTransition());
        MusicLogic();
    }

    private void MusicLogic() {
        Music.ChangeVolume(0);
        Music.SwitchTrackContainer(mainGameTrackContainerIndex);
        Music.ChangeVolumeFaded(0.5f, transitionDuration);
    }

    private IEnumerator ChromeTransition() {
        yield return new WaitForSecondsRealtime(0.02f);
        float piDeflator = Mathf.PI * 0.5f / transitionDuration;
        float timer = 0;

        if (ppVolume.profile.TryGetSettings(out ChromaticAberration chromaticAberration)){
            while (timer < transitionDuration)
            {
                chromaticAberration.intensity.value = (Mathf.Cos(piDeflator * timer) * startCAberration) + (Mathf.Sin(piDeflator * timer) * endCAberration);
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            chromaticAberration.intensity.value = endCAberration;
        }

        startGameEvent.Invoke();
    }
}
