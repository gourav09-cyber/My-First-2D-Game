using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI highScoreTextUI;
    private int highScore = 0;

    public int score = 0;
    public TextMeshProUGUI scoreText;
    private bool gameOver = false;

    void Start()
    {
        // Game shuru hote hi saved high score load kar lo
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreUI();

        if (scoreText != null)
            scoreText.text = "Score: 0";

        InvokeRepeating(nameof(AddScore), 1f, 1f);
    }

    void AddScore()
    {
        if (gameOver) return;

        score++;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        // Jaise-jaise score badhe, check karte raho ki high score to nahi tuta
        CheckHighScore();
    }

    public void CheckHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
            UpdateHighScoreUI();
        }
    }

    void UpdateHighScoreUI()
    {
        if (highScoreTextUI != null)
        {
            highScoreTextUI.text = "High Score: " + highScore;
        }
    }

    public void stopScoring()
    {
        gameOver = true;
        CancelInvoke(nameof(AddScore));
        
        // Game over hote hi ek baar aur final check laga do
        CheckHighScore();
    }
}