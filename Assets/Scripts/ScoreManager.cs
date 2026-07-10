using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI scoreText;
    private bool gameOver = false;

    void Start()
    {
        InvokeRepeating(nameof(AddScore), 1f, 1f);
    }

    void AddScore()
    {
        score++;
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
    
    public void stopScoring()
    {
        CancelInvoke(nameof(AddScore));
    }
}