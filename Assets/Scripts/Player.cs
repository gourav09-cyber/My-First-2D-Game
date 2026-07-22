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
    private bool gameOver = false;

   void Start()
{
    gameOver = false; // Yeh ensure karega ki restart hote hi game over state reset ho jaye
    
    sr = GetComponent<SpriteRenderer>();
    InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);

    if (gameOverText != null)
        gameOverText.gameObject.SetActive(false);
        
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

            // Yahan CoinSpawner ko rokne ke liye Singleton instance ka use kiya hai
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