using TMPro;
using UnityEngine;

public class DisplayFinalScoreLogic : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] TextMeshProUGUI finalScoreText;

    private void FixedUpdate() {
        finalScoreText.SetText(string.Format("Final Score: {0}", scoreManager.playerScore));
    }
}
