using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static int score = 0;
    public TMP_Text scoreText;
    public GameObject gameOverText;

    void Start()
    {
        score = 0;
        scoreText.text = "Score: " + score;
        gameOverText.SetActive(false);
        Time.timeScale = 1f;
    }

    public void AddScore()
    {
        score++;
        scoreText.text = "Score: " + score;

        if (score == 5)
        {
            gameOverText.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}