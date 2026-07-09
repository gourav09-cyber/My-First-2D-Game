using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public AudioClip hitSound; // Drag your hit sound here
    public AudioSource audioSource; // Drag your AudioSource here

    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public float enemyFallSpeed = 2f;

    public TextMeshProUGUI gameOverText; // Drag your TMP Text here

    private bool gameOver = false;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameOver)
        {
            MovePlayer();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void MovePlayer()
    {
        float move = Input.GetAxisRaw("Horizontal");
        transform.Translate(Vector2.right * move * moveSpeed * Time.deltaTime);

        // Optional wrap
        Vector3 pos = transform.position;
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect;

        if (pos.x > screenWidth)
            pos.x = -screenWidth;
        else if (pos.x < -screenWidth)
            pos.x = screenWidth;

        transform.position = pos;
    }

    void SpawnEnemy()
    {
        if (gameOver) return;

        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float randomX = Random.Range(-screenWidth, screenWidth);

        Vector3 spawnPos = new Vector3(
            randomX,
            Camera.main.orthographicSize + 1f,
            0f
        );

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        enemy.tag = "Enemy";

        EnemyMover mover = enemy.AddComponent<EnemyMover>();
        mover.speed = enemyFallSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
            gameOver = true;

            CancelInvoke(nameof(SpawnEnemy));

            // Stop all existing enemies
            foreach (var m in FindObjectsByType<EnemyMover>(FindObjectsSortMode.None))
            {
                m.enabled = false;
            }

            if (gameOverText != null)
                gameOverText.gameObject.SetActive(true);
        }
    }
}

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

