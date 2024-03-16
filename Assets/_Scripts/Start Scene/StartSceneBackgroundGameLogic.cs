using UnityEngine;

public class StartSceneBackgroundGameLogic : MonoBehaviour
{
    [SerializeField] Rigidbody2D ballBody;
    [SerializeField] Ball ball;
    [SerializeField] EnemyAI ai1;
    [SerializeField] EnemyAI ai2;
    [SerializeField] float ballMaxSpeed = 400f;

    private void Start() {
        ballBody.velocity = new Vector2(1, 1).normalized * ball.currentSpeed;
        GliderAudio.Music.SwitchTrackContainer(0);
        GliderAudio.Music.ChangeVolume(1f);
    }

    private void FixedUpdate() {

        ai1.paddleSpeed = Mathf.Abs(ballBody.velocity.y) * Random.Range(1f, 1.25f);
        ai2.paddleSpeed = Mathf.Abs(ballBody.velocity.y) * Random.Range(1.05f, 1.25f);

        ai1.speedLerpStrength = Mathf.Max(Mathf.Abs(ballBody.velocity.y) / 3.2f, 4f);
        ai2.speedLerpStrength = Mathf.Max(Mathf.Abs(ballBody.velocity.y) / 3.5f, 4f);

        ballBody.velocity = new Vector2(Mathf.Sign(ballBody.velocity.x), Mathf.Sign(ballBody.velocity.y)).normalized * ball.currentSpeed;
        ball.currentSpeed = Mathf.Clamp(ball.currentSpeed, 5f, ballMaxSpeed);

        if (ballBody.velocity.x >= 0 && ballBody.velocity.x < 5) ballBody.velocity = new Vector2(5, Mathf.Sqrt(ballBody.velocity.sqrMagnitude - 25) * Mathf.Sign(ballBody.velocity.y));
        if (ballBody.velocity.x < 0 && ballBody.velocity.x > -5) ballBody.velocity = new Vector2(-5, Mathf.Sqrt(ballBody.velocity.sqrMagnitude - 25) * Mathf.Sign(ballBody.velocity.y));
        if (ballBody.velocity.y >= 0 && ballBody.velocity.y < 2) ballBody.velocity = new Vector2(Mathf.Sqrt(ballBody.velocity.sqrMagnitude - 4) * Mathf.Sign(ballBody.velocity.x), 2);
        if (ballBody.velocity.y < 0 && ballBody.velocity.y > -2) ballBody.velocity = new Vector2(Mathf.Sqrt(ballBody.velocity.sqrMagnitude - 4) * Mathf.Sign(ballBody.velocity.x), -2);
    }
}
