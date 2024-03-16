using UnityEngine;
using SOEvents;
using GliderServices;
using GliderSave;

public class GameOverLogic : MonoBehaviour
{
    [SerializeField] IntSaveObject highscoreObject;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] SOEvent gameOverEvent;
    [SerializeField] AnimatedPanel gameOverPanel;
    [SerializeField] string returnToMenuSfx;

    private void Awake() {
        gameOverEvent.AddListener(RunOnGameOver);
    }

    private void RunOnGameOver(){
        gameOverPanel.Open();
        highscoreObject.SetValue(scoreManager.playerScore);
    }

    public void TrySubmitHighscore(){
        AccountSystem accountSystem = FindObjectOfType<AccountSystem>();
        if (accountSystem != null) {
            accountSystem.SubmitHighscore(highscoreObject.GetValue());
            GliderAudio.SFX.PlayStandard(returnToMenuSfx);
        }
        else Debug.Log("Could not find Account System");
    }
}
