using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public float speed = 2f;

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        if (transform.position.y < -Camera.main.orthographicSize - 2f)
        {
            Destroy(gameObject);
        }
    }
}