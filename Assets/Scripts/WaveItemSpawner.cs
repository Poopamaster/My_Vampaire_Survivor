using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WaveItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ItemSpawnData
    {
        public GameObject itemPrefab; // prefab ของ item เช่น CircleSwordPickup
        public int minPerWave = 0;
        public int maxPerWave = 2;
    }

    [Header("Item Spawn Settings")]
    public List<ItemSpawnData> itemsToSpawn;
    public Transform player;           // reference ไปยัง Player
    public float spawnRadius = 10f;    // รัศมีรอบๆ player ที่จะสุ่มเกิด
    public LayerMask groundMask;       // สำหรับวางบนพื้น
    public float itemHeight = 0.5f;    // ความสูงจากพื้นเวลาสร้าง item

    [Header("Wave Sync")]
    public EnemyWaveSpawner waveSpawner; // อ้างอิงระบบ wave ของคุณ

    private int currentWave = 0;

    void Start()
    {
        if (waveSpawner == null)
        {
            waveSpawner = FindObjectOfType<EnemyWaveSpawner>();
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // สมัคร event ถ้ามีระบบ wave event (หรือคุณจะเรียก SpawnItemsPerWave() จาก EnemyWaveSpawner ก็ได้)
        StartCoroutine(CheckWaveProgress());
    }

    IEnumerator CheckWaveProgress()
    {
        while (true)
        {
            if (waveSpawner != null && waveSpawner.waveNumber > currentWave)
            {
                currentWave = waveSpawner.waveNumber;
                SpawnItemsPerWave(currentWave);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    void SpawnItemsPerWave(int wave)
    {
        Debug.Log($"🎁 Spawning upgrade items for Wave {wave}");

        foreach (var item in itemsToSpawn)
        {
            int spawnCount = Random.Range(item.minPerWave, item.maxPerWave + 1);
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPos = GetRandomPositionAroundPlayer();
                GameObject newItem = Instantiate(item.itemPrefab, spawnPos, Quaternion.identity);
                Debug.Log($"🪄 Spawned {newItem.name} at {spawnPos}");
            }
        }
    }

    Vector3 GetRandomPositionAroundPlayer()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(3f, spawnRadius);
        Vector3 pos = new Vector3(randomCircle.x, 10f, randomCircle.y) + player.position;

        // raycast หาพื้น
        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 20f, groundMask))
        {
            pos.y = hit.point.y + itemHeight;
        }
        else
        {
            pos.y = player.position.y + itemHeight;
        }

        return pos;
    }
}
