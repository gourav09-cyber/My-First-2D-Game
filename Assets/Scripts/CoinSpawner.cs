using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public static CoinSpawner Instance;

    public GameObject coinPrefab;
    public float spawnRate = 1f;
    public float minX = -8f;
    public float maxX = 8f;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        // Scene restart hone par spawning dobara start ho jayegi
        InvokeRepeating(nameof(SpawnCoin), 2f, spawnRate);
    }

    public void StopSpawning()
    {
        CancelInvoke(nameof(SpawnCoin));
    }

    void SpawnCoin()
    {
        float randomX = Random.Range(minX, maxX);
        Vector2 spawnPosition = new Vector2(randomX, 6f);
        Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
    }
}