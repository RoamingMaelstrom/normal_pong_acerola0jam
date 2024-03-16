using System.Collections;
using UnityEngine;

public class CameraPanningLogic : MonoBehaviour
{
    [SerializeField] float minZoom = 6f;
    [SerializeField] float maxZoom = 16f;
    [SerializeField] Bounds cameraPosBounds;
    [SerializeField] float minTime = 8f;
    [SerializeField] float maxTime = 16f;

    private float timer = 0f;

    private Coroutine PanRoutine;

    [SerializeField] float maxDelta = 0;

    private void FixedUpdate() {
        timer -= Time.fixedDeltaTime;
        if (timer <= 0) {
            if (PanRoutine != null) StopCoroutine(PanRoutine);
            PanRoutine = StartCoroutine(CameraPan());
        }
            
    }

    private IEnumerator CameraPan()
    {
        minZoom = Mathf.Min(minZoom * 1.02f, maxZoom - 0.1f);
        maxZoom = Mathf.Min(maxZoom + 0.1f, 19f);
        Vector3 startPos = Camera.main.transform.position;
        Vector3 targetPos;

        int c = 0;
        do
        {
            c++;
            targetPos = new Vector3(Random.Range(cameraPosBounds.min.x, cameraPosBounds.max.x), Random.Range(cameraPosBounds.min.y, cameraPosBounds.max.y), -10f);
        } while (c < 25 && (targetPos - startPos).magnitude < cameraPosBounds.size.magnitude * 0.4f);


        float startZoom = Camera.main.orthographicSize;
        float targetZoom = Random.Range(minZoom, maxZoom);

        float duration = Random.Range(minTime, maxTime);
        timer = duration;

        while(timer > 0)
        {
            float before = Camera.main.orthographicSize;

            Camera.main.transform.position = Vector3.Lerp(startPos, targetPos, (duration - timer) / duration);
            Camera.main.orthographicSize = Mathf.Lerp(startZoom, targetZoom, (duration - timer) / duration);

            if (Mathf.Abs(before - Camera.main.orthographicSize) > maxDelta){
                maxDelta = Mathf.Abs(before - Camera.main.orthographicSize);
            }

            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        timer = 0;
    }
}
