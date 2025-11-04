using System.Collections;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs;     // ศัตรูที่ใช้ spawn ได้ (ใส่ได้หลายตัว เช่น melee, ranged)
    public Transform player;              // ตัว player ที่ศัตรูจะวิ่งไปหา
    public float spawnRadius = 20f;       // รัศมีรอบ ๆ player ที่จะสุ่มตำแหน่ง spawn
    public int startEnemies = 5;          // จำนวนศัตรูเริ่มต้นใน Wave 1
    public float spawnDelay = 0.3f;       // หน่วงเวลาระหว่าง spawn แต่ละตัว

    [Header("Wave Settings")]
    public float timeBetweenWaves = 5f;   // เวลาพักระหว่าง Wave
    public int waveNumber = 0;            // หมายเลข Wave ปัจจุบัน
    public float difficultyMultiplier = 1.2f; // คูณความยากของศัตรูต่อ Wave

    private bool spawning = false;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(2f); // หน่วงเริ่มเกมเล็กน้อย

        while (true)
        {
            waveNumber++;
            int enemyCount = Mathf.RoundToInt(startEnemies * Mathf.Pow(difficultyMultiplier, waveNumber - 1));

            Debug.Log($"🌀 Wave {waveNumber} started! Spawning {enemyCount} enemies...");

            spawning = true;
            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnDelay);
            }
            spawning = false;

            // รอจนกว่าศัตรูจะหมดก่อนเริ่ม Wave ถัดไป
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

            Debug.Log($"✅ Wave {waveNumber} cleared!");
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0) return;

        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Vector3 spawnPos = RandomSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemy.tag = "Enemy";

        EnemyController e = enemy.GetComponent<EnemyController>();
        if (e != null)
        {
            e.moveSpeed *= Mathf.Pow(difficultyMultiplier, waveNumber - 1);
            e.health *= Mathf.Pow(difficultyMultiplier, waveNumber - 1);
        }
    }

    Vector3 RandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 pos = new Vector3(randomCircle.x, 0f, randomCircle.y);
        pos += player.position;
        return pos;
    }
}
