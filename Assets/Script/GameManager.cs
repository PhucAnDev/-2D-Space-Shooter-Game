using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;
    public float minInstantiateValue = -8f;
    public float maxInstantiateValue = 9f;
    public float enemyDestroyTime = 10f;

    [Header("Enemy1")]
    public GameObject enemyPrefab1;
    public float minInstantiateValue1 = -8f;
    public float maxInstantiateValue1 = 9f;
    public float enemyDestroyTime1 = 10f;

    [Header("Enemy toughness scaling")]
    public int baseHitsToDie = 2;         // lúc đầu cần 2 hit
    public float hitsRampEverySec = 20f;  // mỗi 20s tăng thêm 1 hit
    public int maxHitsToDie = 10;         // (tuỳ chọn) trần trên

    [Header("Spawn pacing")]
    public float startSpawnInterval = 1.5f;   // lúc ð?u: 1.5s 1 con
    public float minSpawnInterval = 0.25f;  // nhanh nh?t: 0.25s 1 con
    public float spawnAccelPerSec = 0.02f;  // m?i giây gi?m 0.02s (t?i min)

    [Header("Enemy speed scaling")]
    public float baseEnemySpeed = 1.0f;   // t?c ð? rõi lúc ð?u
    public float speedRampPerSec = 0.02f;  // m?i giây tãng thêm 0.02 (tu? ch?nh)

    private float elapsed;                    // th?i gian trôi qua

    [Header("Coin")]
    public GameObject coinPrefab;
    public float coinSpawnInterval = 5f;
    public float coinDestroyTime = 8f;
    public float coinFallSpeed = 0.5f;

    [Header("Energy Orb")]
    public GameObject energyOrbPrefab;
    public float energyOrbSpawnInterval = 10f;
    public float energyOrbDestroyTime = 8f;

  

    IEnumerator SpawnEnergyOrbs()
    {
        while (true)
        {
            Vector3 pos = new Vector3(Random.Range(-7f, 7f), 6f, 0f);
            GameObject orb = Instantiate(energyOrbPrefab, pos, Quaternion.identity);
            Destroy(orb, energyOrbDestroyTime);
            yield return new WaitForSeconds(energyOrbSpawnInterval);
        }
    }

    void Start()
    {
        StartCoroutine(SpawnLoop());
        StartCoroutine(SpawnLoop1());
        StartCoroutine(SpawnCoins()); 
        StartCoroutine(SpawnEnergyOrbs());
    }

    IEnumerator SpawnLoop()
    {
        float interval = startSpawnInterval;

        while (true)
        {
            float currentSpeed = baseEnemySpeed + elapsed * speedRampPerSec;
            interval = Mathf.Max(minSpawnInterval, startSpawnInterval - elapsed * spawnAccelPerSec);

            Vector3 pos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 6f, 0f);
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.Euler(0f, 0f, 180f));

            var ec = enemy.GetComponent<EnemyController>();
            if (ec != null)
            {
                ec.speed = currentSpeed;

                // Tính số hit cần để chết dựa theo thời gian đã trôi
                int extra = Mathf.FloorToInt(elapsed / hitsRampEverySec); // ví dụ 0..1..2..
                ec.hitsToDie = Mathf.Clamp(baseHitsToDie + extra, baseHitsToDie, maxHitsToDie);
            }

            Destroy(enemy, enemyDestroyTime);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }

    IEnumerator SpawnLoop1()
    {
        float interval = startSpawnInterval;

        while (true)
        {
            float currentSpeed = baseEnemySpeed + elapsed * speedRampPerSec;
            interval = Mathf.Max(minSpawnInterval, startSpawnInterval - elapsed * spawnAccelPerSec);

            Vector3 pos = new Vector3(Random.Range(minInstantiateValue1, maxInstantiateValue1), 6f, 0f);
            GameObject enemy = Instantiate(enemyPrefab1, pos, Quaternion.Euler(0f, 0f, 180f));

            var ec = enemy.GetComponent<EnemyController>();
            if (ec != null)
            {
                ec.speed = currentSpeed;

                // Tính số hit cần để chết dựa theo thời gian đã trôi
                int extra = Mathf.FloorToInt(elapsed / hitsRampEverySec); // ví dụ 0..1..2..
                ec.hitsToDie = Mathf.Clamp(baseHitsToDie + extra, baseHitsToDie, maxHitsToDie);
            }

            Destroy(enemy, enemyDestroyTime1);
            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }
    }
    IEnumerator SpawnCoins()
    {
        while (true)
        {
            Vector3 pos = new Vector3(Random.Range(-7f, 9f), 6f, 0f);
            GameObject coin = Instantiate(coinPrefab, pos, Quaternion.identity);
            CoinSpawn cc = coin.GetComponent<CoinSpawn>();
            if (cc != null)
                cc.fallSpeed = coinFallSpeed;

            Destroy(coin, coinDestroyTime);
            yield return new WaitForSeconds(coinSpawnInterval);
        }
    }
}
