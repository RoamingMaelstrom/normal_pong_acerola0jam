using System.Collections;
using SOEvents;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] IntSOEvent playerWinRoundEvent;
    [SerializeField] IntSOEvent playerLoseRoundEvent;
    [SerializeField] SOEvent startGameEvent;
    [SerializeField] Modifier modifierClass;
    [SerializeField] Ball ball;
    [SerializeField] Rigidbody2D ballBody;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] PlayerHealth playerHealth;
    public float ballStartSpeed = 10f;
    [SerializeField] float passiveBallStartSpeedGain = 1.0f;
    public float playerDirectionSpeedMultiplier = 1f;
    public float opponentDirectionSpeedMultiplier = 1f;
    [SerializeField] float ballSpawnYVariance = 6f;
    public int ballDamage = 1;
    public int ballDelayMs = 1500;
    [SerializeField] string ballStartMovementSfx;
    [SerializeField] string playerwinSfx;
    [SerializeField] string negateSfx;

    public bool flippedScreen = false;

    private void Awake() {
        startGameEvent.AddListener(OnGameStart);
    }

    private void OnGameStart() {
        ResetBall(WinStatus.NONE);
    }
    
    private void FixedUpdate() {
        if (!ballBody.simulated) return;

        Vector3 ballPosScreen = Camera.main.WorldToScreenPoint(ballBody.position);
        if (ballPosScreen.x > Screen.width) ResetBall(flippedScreen ? WinStatus.OPPONENT : WinStatus.PLAYER);
        else if (ballPosScreen.x < 0) ResetBall(flippedScreen ? WinStatus.PLAYER : WinStatus.OPPONENT);

        ballBody.velocity = ballBody.velocity.normalized * ball.currentSpeed;
        if (ballBody.velocity.magnitude == 0) return;
        
        if (ballBody.velocity.x >= 0 && ballBody.velocity.x < 5) ballBody.velocity = new Vector2(5, Mathf.Sqrt(ballBody.velocity.sqrMagnitude - 25) * Mathf.Sign(ballBody.velocity.y));
        if (ballBody.velocity.x < 0 && ballBody.velocity.x > -5) ballBody.velocity = new Vector2(-5, Mathf.Sqrt(ballBody.velocity.sqrMagnitude - 25) * Mathf.Sign(ballBody.velocity.y));
        if (ballBody.velocity.y >= 0 && ballBody.velocity.y < 2) ballBody.velocity = new Vector2(Mathf.Sqrt(ballBody.velocity.sqrMagnitude - 4) * Mathf.Sign(ballBody.velocity.x), 2);
        if (ballBody.velocity.y < 0 && ballBody.velocity.y > -2) ballBody.velocity = new Vector2(Mathf.Sqrt(ballBody.velocity.sqrMagnitude - 4) * Mathf.Sign(ballBody.velocity.x), -2);


        if (ballBody.velocity.x > 0) {
            ballBody.velocity *= playerDirectionSpeedMultiplier;
            ballBody.gameObject.layer = LayerMask.NameToLayer("Ball Player");
        }
        if (ballBody.velocity.x < 0) {
            ballBody.velocity *= opponentDirectionSpeedMultiplier;
            ballBody.gameObject.layer = LayerMask.NameToLayer("Ball Opponent");
        }
    }

    private void ResetBall(WinStatus winStatus) {
        playerHealth.TickPassiveHealing(winStatus);
        UpdateScores(winStatus);
        ResetBallPosition();
        ballBody.velocity = Vector2.zero;
        ball.Freeze();
        ballDelayMs = (int)(ballDelayMs * 0.98f);
        StartCoroutine(MoveBallDelayed(ballDelayMs * (winStatus == WinStatus.NONE ? 2 : 1)));
    }

    private void ResetBallPosition(){
        bool collision;
        float yPos;
        int c = 0;

        do
        {
            collision = false;
            c++;
            yPos = Random.Range(-ballSpawnYVariance, ballSpawnYVariance);
            if ((modifierClass.validObstaclePlaces & 1) > 0 && yPos > -12f && yPos < -7f) collision = true;
            else if ((modifierClass.validObstaclePlaces & 2) > 0 && yPos > -2.5f && yPos < 2.5f) collision = true;
            else if ((modifierClass.validObstaclePlaces & 4) > 0 && yPos > 7f && yPos < 12f) collision = true;

        } while (collision && c < 50);

        ballBody.transform.position = Vector3.zero + (Vector3.up * yPos);
    }

    private IEnumerator MoveBallDelayed(float delayMs) {
        yield return new WaitForSeconds(delayMs / 1000f);

        float startX = Random.Range(-10f, 10f);
        if (Mathf.Abs(startX) < 0.001f) startX = 0.02f;
        float startY = startX * (Random.Range(0.85f, 1.4f) * Mathf.Sign(Random.Range(-1f, 1f)));
        
        float startSpeed = ballStartSpeed + (passiveBallStartSpeedGain * scoreManager.round);

        ball.UnFreeze();
        ballBody.velocity = new Vector2(startX, startY).normalized * startSpeed;
        ball.currentSpeed = startSpeed;
        GliderAudio.SFX.PlayStandard(ballStartMovementSfx);
    }

    private void UpdateScores(WinStatus winStatus) {
        scoreManager.round ++;
        switch (winStatus){
            case WinStatus.PLAYER: 
            {
                scoreManager.playerScore ++;
                GliderAudio.SFX.PlayStandard(playerwinSfx);
                playerWinRoundEvent.Invoke(scoreManager.playerScore);
                break;
            }
            case WinStatus.OPPONENT: 
            {
                scoreManager.opponentScore ++; 
                playerLoseRoundEvent.Invoke(scoreManager.opponentScore);
                
                if (playerHealth.NegateDamage()){
                    GliderAudio.SFX.PlayStandard(negateSfx);
                    return;
                }
                playerHealth.DecreaseHealth(ballDamage);
                break;
            }
            default: break;
        }
    }
}


public enum WinStatus{
    NONE = 0,
    PLAYER = 1,
    OPPONENT = 2
}
