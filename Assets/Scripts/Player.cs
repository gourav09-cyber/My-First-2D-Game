using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    public TextMeshProUGUI coinTextUI;
    private int coinCount = 0;         

    public float moveSpeed = 5f;
    public AudioClip hitSound; 
    public AudioSource audioSource; 
    private SpriteRenderer sr;

    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public float enemyFallSpeed = 2f;

    public TextMeshProUGUI gameOverText; 
    public GameObject restartButton; // Restart button ka reference
    private bool gameOver = false;
    private float mobileMoveInput = 0f; // Mobile buttons ke liye input variable

    void Start()
    {
        gameOver = false; 
        
        sr = GetComponent<SpriteRenderer>();
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        if (restartButton != null)
            restartButton.SetActive(false); // Game shuru hote hi restart button hide rahega
            
        if (coinTextUI != null)
            coinTextUI.text = "Coins: 0";
    }

    void Update()
    {
        if (!gameOver)
        {
            MovePlayer();
        }
        else
        {
            // Keyboard shortcut 'R' ya PC ke liye
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }
    
    void MovePlayer()
    {
        float move = Input.GetAxisRaw("Horizontal");
        if (move == 0f)
        {
            move = mobileMoveInput; // Agar keyboard nahi daba, toh mobile touch input lega
        }

        transform.Translate(Vector2.right * move * moveSpeed * Time.deltaTime);

        if (move > 0)
            sr.flipX = false;
        else if (move < 0)
            sr.flipX = true;

        Vector3 pos = transform.position;
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect;

        if (pos.x > screenWidth)
            pos.x = -screenWidth;
        else if (pos.x < -screenWidth)
            pos.x = screenWidth;

        transform.position = pos;
    }

    // Mobile buttons ke liye public functions
    public void MoveLeft()
    {
        mobileMoveInput = -1f;
    }

    public void MoveRight()
    {
        mobileMoveInput = 1f;
    }

    public void StopMoving()
    {
        mobileMoveInput = 0f;
    }

    // Restart button ya UI se call karne ke liye function
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

            if (CoinSpawner.Instance != null)
            {
                CoinSpawner.Instance.StopSpawning();
            }

            foreach (var m in FindObjectsByType<EnemyMover>(FindObjectsInactive.Exclude))
            {
                m.enabled = false;
            }
            
            FindFirstObjectByType<ScoreManager>()?.stopScoring();

            if (gameOverText != null)
                gameOverText.gameObject.SetActive(true);

            if (restartButton != null)
                restartButton.SetActive(true); // Game over hone par restart button show ho jayega
        }

        if (collision.CompareTag("Coin"))
        {
            Destroy(collision.gameObject); 
            
            coinCount++; 
            
            if (coinTextUI != null)
            {
                coinTextUI.text = "Coins: " + coinCount;
            }
            
            Debug.Log("Coin Collected: " + coinCount); 
        }
    }
}