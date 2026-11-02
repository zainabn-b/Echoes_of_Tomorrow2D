using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform[] obstacles;
    public float moveTime = 1f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= moveTime)
        {
            timer = 0f;

            if (obstacles.Length > 0)
            {
                int randomIndex = Random.Range(0, obstacles.Length);
                transform.position = new Vector3(
                    obstacles[randomIndex].position.x,
                    obstacles[randomIndex].position.y + 1f,
                    transform.position.z
                );
            }
        }
    }
}