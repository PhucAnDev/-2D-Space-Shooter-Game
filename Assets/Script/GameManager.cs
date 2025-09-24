using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;
    public float minInstantiateValue = -7f;
    public float maxInstantiateValue = 7f;
    public float enemyDestroyTime = 10f;

    [Header("Spawn pacing")]
    public float startSpawnInterval = 1.5f;   // l�c �?u: 1.5s 1 con
    public float minSpawnInterval = 0.25f;  // nhanh nh?t: 0.25s 1 con
    public float spawnAccelPerSec = 0.02f;  // m?i gi�y gi?m 0.02s (t?i min)

    [Header("Enemy speed scaling")]
    public float baseEnemySpeed = 1.0f;   // t?c �? r�i l�c �?u
    public float speedRampPerSec = 0.02f;  // m?i gi�y t�ng th�m 0.02 (tu? ch?nh)

    private float elapsed;                    // th?i gian tr�i qua

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        float interval = startSpawnInterval;

        while (true)
        {
            // th?i �i?m hi?n t?i
            float currentSpeed = baseEnemySpeed + elapsed * speedRampPerSec;
            interval = Mathf.Max(minSpawnInterval, startSpawnInterval - elapsed * spawnAccelPerSec);

            // t?o enemy
            Vector3 pos = new Vector3(Random.Range(minInstantiateValue, maxInstantiateValue), 6f, 0f);
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.Euler(0f, 0f, 180f));

            // g�n t?c �? r�i t�ng d?n
            var ec = enemy.GetComponent<EnemyController>();
            if (ec != null) ec.speed = currentSpeed;

            // t? hu? sau X gi�y
            Destroy(enemy, enemyDestroyTime);

            // ch? theo interval hi?n t?i
            yield return new WaitForSeconds(interval);

            // c?ng elapsed theo ��ng th?i gian �? ch?
            elapsed += interval;
        }
    }
}
