using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    public GameObject gameOverText;

    private void EndGame()
    {
        gameOverText.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EndGame();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EndGame();
        }
    }
}