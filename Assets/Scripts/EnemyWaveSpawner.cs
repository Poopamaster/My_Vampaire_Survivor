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
    public GameObject spawnArea;
    public float spawnHeightOffset = 0.2f;

    [Header("Wave Settings")]
    public int totalRounds = 15;
    public float roundDuration = 45f;
    public float breakDuration = 3f;
    public float difficultyMultiplier = 1.18f;

    [Header("UI Reference")]
    public WaveUI waveUI;

    [Header("Status (Read Only)")]
    public int currentRound = 0;
    public bool isSpawning = false;
    public float currentRoundTimeRemaining = 0f;
    public float currentBreakTimeRemaining = 0f;

    public bool canDropItem = true;
    private Bounds areaBounds;
    private Coroutine roundCoroutine;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (spawnArea != null)
        {
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

        if (waveUI == null)
            waveUI = FindObjectOfType<WaveUI>();

        StartWaveSystem();
    }

    void Update()
    {
        // อัพเดทเวลาเหลือสำหรับ UI
        if (isSpawning)
        {
            currentRoundTimeRemaining -= Time.deltaTime;
            currentRoundTimeRemaining = Mathf.Max(0f, currentRoundTimeRemaining);
        }
        else if (currentRound < totalRounds && currentRound > 0)
        {
            currentBreakTimeRemaining -= Time.deltaTime;
            currentBreakTimeRemaining = Mathf.Max(0f, currentBreakTimeRemaining);
        }
    }

    public void StartWaveSystem()
    {
        if (roundCoroutine != null)
            StopCoroutine(roundCoroutine);
        
        roundCoroutine = StartCoroutine(RoundLoop());
    }

    IEnumerator RoundLoop()
    {
        yield return new WaitForSeconds(2f);

        for (currentRound = 1; currentRound <= totalRounds; currentRound++)
        {
            // เริ่ม Wave ใหม่
            isSpawning = true;
            currentRoundTimeRemaining = roundDuration;
            currentBreakTimeRemaining = 0f;

            // เริ่ม Spawn ศัตรูทั้งหมดในกลุ่ม
            foreach (var group in enemyGroups)
                StartCoroutine(SpawnEnemyGroup(group));

            // รอจนกว่าเวลาของ Wave จะหมด
            yield return new WaitForSeconds(roundDuration);

            isSpawning = false;
            ClearAllEnemies();
            
            // ถ้ายังไม่ใช่ Wave สุดท้าย ให้พัก
            if (currentRound < totalRounds)
            {
                currentBreakTimeRemaining = breakDuration;
                yield return new WaitForSeconds(breakDuration);
            }
        }

        // จบเกม
        isSpawning = false;
        currentRoundTimeRemaining = 0f;
        currentBreakTimeRemaining = 0f;
        
        Debug.Log("All waves completed!");
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

    // Method สำหรับดึงสถานะปัจจุบัน (ใช้โดย UI)
    public bool IsInBreakTime()
    {
        return !isSpawning && currentRound < totalRounds && currentRound > 0;
    }

    public float GetRoundTimeRemaining()
    {
        return currentRoundTimeRemaining;
    }

    public float GetBreakTimeRemaining()
    {
        return currentBreakTimeRemaining;
    }
}