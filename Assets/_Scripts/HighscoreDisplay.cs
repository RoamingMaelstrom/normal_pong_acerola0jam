using UnityEngine;
using GliderSave;
using TMPro;

public class HighscoreDisplay : MonoBehaviour
{
    [SerializeField] IntSaveObject highscoreSaveObject;
    [SerializeField] TextMeshProUGUI highscoreText;

    private void FixedUpdate() {
        highscoreText.SetText(string.Format("Highscore: {0}", highscoreSaveObject.GetValue()));
    }
}
