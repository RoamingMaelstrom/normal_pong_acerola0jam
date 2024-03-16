using UnityEngine;
using TMPro;
using SOEvents;
using System.Collections;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] IntSOEvent playerWinRoundEvent;
    [SerializeField] IntSOEvent playerLoseRoundEvent;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] TextMeshProUGUI playerScoreText;
    [SerializeField] FlashAnimationText playerScoreTextAnimation;
    [SerializeField] TextMeshProUGUI opponentScoreText;
    [SerializeField] FlashAnimationText opponentScoreTextAnimation;

    private void Awake() {
        playerWinRoundEvent.AddListener(FlashPlayerScore);
        playerLoseRoundEvent.AddListener(FlashOpponentScore);
    }

    private void FlashOpponentScore(int arg0) {
         opponentScoreTextAnimation.PlayFlashingAnimation();
    }

    private void FlashPlayerScore(int arg0) {
        playerScoreTextAnimation.PlayFlashingAnimation();
    }

    private void FixedUpdate() {
        playerScoreText.SetText(scoreManager.playerScore.ToString());
        opponentScoreText.SetText(scoreManager.opponentScore.ToString());
    }
}

