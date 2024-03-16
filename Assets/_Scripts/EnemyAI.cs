using SOEvents;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] IntSOEvent playerWinRoundEvent;
    public Rigidbody2D paddleBody;
    [SerializeField] Rigidbody2D ballBody;
    public float paddleSpeed;
    public float speedLerpStrength = 1.5f;
    [SerializeField] float velocityAccount = 0.25f;
    [SerializeField] float paddleSpeedGainPlayerVictory = 0.5f;
    [SerializeField] float lerpStrengthGainPlayerVictory = 0.05f;
    public bool aiActive = true;

    private void Awake() {
        playerWinRoundEvent.AddListener(IncreaseSpeed);
    }

    private void IncreaseSpeed(int arg0)
    {
        paddleSpeed += paddleSpeedGainPlayerVictory;
        speedLerpStrength += lerpStrengthGainPlayerVictory;
    }

    private void FixedUpdate() {
        if (!aiActive) return;
        float deltaY = paddleBody.position.y - (ballBody.position.y + (ballBody.velocity.y * velocityAccount));
        float currentSpeed = paddleBody.velocity.y;
        if (deltaY > 0.1f) paddleBody.velocity = new Vector2(0f, Mathf.Lerp(currentSpeed, -paddleSpeed, speedLerpStrength * Time.fixedDeltaTime));
        if (deltaY < -0.1f) paddleBody.velocity = new Vector2(0f, Mathf.Lerp(currentSpeed, paddleSpeed, speedLerpStrength * Time.fixedDeltaTime));
    }

    public void SetPaddle(Rigidbody2D newPaddle){
        newPaddle.gameObject.SetActive(true);
        newPaddle.transform.position = paddleBody.transform.position;
        newPaddle.velocity = paddleBody.velocity;
        paddleBody.gameObject.SetActive(false);
        paddleBody = newPaddle;
    }
}
