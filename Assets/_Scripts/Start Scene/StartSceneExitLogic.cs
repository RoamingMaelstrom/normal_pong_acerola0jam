using UnityEngine;
using GliderAudio;
using UnityEngine.EventSystems;
using System.Collections;

public class StartSceneExitLogic : MonoBehaviour
{
    [SerializeField] string sceneExitBlipSfx;
    [SerializeField] string sceneTransitionRiserSfx;
    [SerializeField] string selectButtonSfx;
    [SerializeField] string deselectButtonSfx;

    public void EventSystemDeselect() {
        StartCoroutine(DeselectDelayed(0.05f));
    }

    private IEnumerator DeselectDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void RunSoundLogicOnSceneExit() {
        SFX.PlayStandard(sceneExitBlipSfx);
        SFX.PlayStandard(sceneTransitionRiserSfx);
        Music.ChangeVolumeFaded(0f, 1f);
    }

    public void ToggleObject(GameObject obj) {
        if (obj.activeInHierarchy) SFX.PlayStandard(deselectButtonSfx);
        else SFX.PlayStandard(selectButtonSfx);
        obj.SetActive(!obj.activeInHierarchy);
    }
}
