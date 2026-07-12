using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    // --- Naye Coin Variables ---
    public TextMeshProUGUI coinTextUI; // Score dikhane ke liye UI
    private int coinCount = 0;         // Coin ginnne ke liye

    // --- Purane Variables ---
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
        sr = GetComponent<SpriteRenderer>();
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
            
        // Start me coin text ko 0 set kar dete hain (agar UI link kiya hoga)
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
        // --- ENEMY (METEOR) WALA LOGIC ---
        if (collision.CompareTag("Enemy"))
        {
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
            gameOver = true;

            // Meteors ko aana roko
            CancelInvoke(nameof(SpawnEnemy));

            // NAYI LINE: Coins ko aana roko
            FindFirstObjectByType<CoinSpawner>()?.CancelInvoke("SpawnCoin");

            // Jo meteors pehle se screen par hain, unko roko
            foreach (var m in FindObjectsByType<EnemyMover>(FindObjectsInactive.Exclude))
            {
                m.enabled = false;
            }
            
            FindFirstObjectByType<ScoreManager>()?.stopScoring();

            if (gameOverText != null)
                gameOverText.gameObject.SetActive(true);
        }

        // --- NAYA COIN WALA LOGIC ---
        if (collision.CompareTag("Coin"))
        {
            // 1. Coin ko gayab karo
            Destroy(collision.gameObject); 
            
            // 2. Coin ka number badhao
            coinCount++; 
            
            // 3. UI par naya number dikhao
            if (coinTextUI != null)
            {
                coinTextUI.text = "Coins: " + coinCount;
            }
            
            Debug.Log("Coin Collected: " + coinCount); 
        }
    }
}