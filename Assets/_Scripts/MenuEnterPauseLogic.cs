using System.Collections;
using SOEvents;
using UnityEngine;

public class MenuEnterPauseLogic : MonoBehaviour
{
    [SerializeField] IntSOEvent playerWinRoundEvent;
    [SerializeField] SOEvent gameOverEvent;
    [SerializeField] BoolSOEvent updatePauseCounterEvent;

    private void Awake() {
        playerWinRoundEvent.AddListener(EnterPause);
        gameOverEvent.AddListener(EnterPause);
    }

    private void EnterPause(int arg0)
    {
        StartCoroutine(EnterPauseDelayed(Time.fixedDeltaTime));
    }

    private void EnterPause()
    {
        StartCoroutine(EnterPauseDelayed(Time.fixedDeltaTime));
    }

    private IEnumerator EnterPauseDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        updatePauseCounterEvent.Invoke(true);
    }
}
