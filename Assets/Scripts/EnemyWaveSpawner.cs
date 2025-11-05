using System.Collections;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroupData
    {
        public GameObject enemyPrefab;
        public int groupSize = 5;
        public float spawnInterval = 3f;
    }

    [Header("Spawn Settings")]
    public EnemyGroupData[] enemyGroups;
    public Transform player;
    public GameObject spawnArea; // ✅ วัตถุ Plane หลัก
    public float spawnHeightOffset = 0.2f; // เผื่อให้ศัตรูลอยจากพื้นเล็กน้อย

    [Header("Wave Settings")]
    public int totalRounds = 15;
    public float roundDuration = 45f;
    public float breakDuration = 3f;
    public float difficultyMultiplier = 1.18f;

    [Header("Status (Read Only)")]
    public int currentRound = 0;
    public bool isSpawning = false;

    public bool canDropItem = true;
    private Bounds areaBounds;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (spawnArea != null)
        {
            // ✅ ดึงขนาดของ plane
            Renderer rend = spawnArea.GetComponent<Renderer>();
            Collider col = spawnArea.GetComponent<Collider>();
            if (rend != null)
                areaBounds = rend.bounds;
            else if (col != null)
                areaBounds = col.bounds;
        }
        else
        {
            Debug.LogWarning("⚠️ spawnArea ยังไม่ถูกกำหนดใน Inspector");
        }

        StartCoroutine(RoundLoop());
    }

    IEnumerator RoundLoop()
    {
        yield return new WaitForSeconds(2f);

        for (currentRound = 1; currentRound <= totalRounds; currentRound++)
        {
            isSpawning = true;

            foreach (var group in enemyGroups)
                StartCoroutine(SpawnEnemyGroup(group));

            yield return new WaitForSeconds(roundDuration);

            isSpawning = false;
            ClearAllEnemies();
            yield return new WaitForSeconds(breakDuration);
        }
    }

    IEnumerator SpawnEnemyGroup(EnemyGroupData group)
    {
        while (isSpawning)
        {
            for (int i = 0; i < group.groupSize; i++)
            {
                SpawnEnemy(group.enemyPrefab);
                yield return new WaitForSeconds(0.15f);
            }

            yield return new WaitForSeconds(group.spawnInterval);
        }
    }

    void SpawnEnemy(GameObject prefab)
    {
        if (!prefab || spawnArea == null) return;

        Vector3 spawnPos = RandomPointInPlane();
        spawnPos.y += spawnHeightOffset;

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        enemy.tag = "Enemy";

        EnemyController e = enemy.GetComponent<EnemyController>();
        if (e != null)
        {
            float diff = Mathf.Pow(difficultyMultiplier, currentRound - 1);
            e.moveSpeed *= diff;
            e.health *= diff;
        }
    }

    Vector3 RandomPointInPlane()
    {
        // ✅ สุ่มตำแหน่งในขอบ plane
        float x = Random.Range(areaBounds.min.x, areaBounds.max.x);
        float z = Random.Range(areaBounds.min.z, areaBounds.max.z);
        float y = areaBounds.center.y;

        return new Vector3(x, y, z);
    }

    void ClearAllEnemies()
    {
        foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
            Destroy(e);
    }
}
