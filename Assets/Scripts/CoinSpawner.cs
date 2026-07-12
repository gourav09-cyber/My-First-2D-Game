using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public float spawnRate = 3f; // Har 3 second mein 1 coin aayega
    public float minX = -8f; // Screen ka left kinara
    public float maxX = 8f;  // Screen ka right kinara

    void Start()
    {
        // Game start hote hi coin tapkana shuru karo
        InvokeRepeating(nameof(SpawnCoin), 2f, spawnRate);
    }

    void SpawnCoin()
    {
        // Random X position decide karo
        float randomX = Random.Range(minX, maxX);
        Vector2 spawnPosition = new Vector2(randomX, 6f); // 6f matlab screen ke upar (aasman mein)

        // Coin ko scene mein paida karo
        Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
    }
}