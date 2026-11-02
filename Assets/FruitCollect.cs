using UnityEngine;

public class Fruit : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !collected)
        {
            collected = true;

            FindFirstObjectByType<ScoreManager>().AddScore();

            Destroy(gameObject);
        }
    }
}