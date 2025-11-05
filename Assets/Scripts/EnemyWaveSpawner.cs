using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public float spawnRadius = 20f;

    [Header("Wave Settings")]
    public int totalRounds = 15;
    public float roundDuration = 45f;
    public float breakDuration = 3f;
    [Tooltip("คูณความยากต่อรอบ (ค่าแนะนำ 1.15 - 1.25)")]
    public float difficultyMultiplier = 1.18f;

    [Header("Status (Read Only)")]
    public int currentRound = 0;
    public bool isSpawning = false;
    public bool canDropItem = true; // 🔹 ใช้ควบคุมไม่ให้ดรอประหว่างเวลาลบศัตรูตอนจบ Wave

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(RoundLoop());
    }

    IEnumerator RoundLoop()
    {
        yield return new WaitForSeconds(2f);

        for (currentRound = 1; currentRound <= totalRounds; currentRound++)
        {
            Debug.Log($"🌀 Round {currentRound} started!");
            isSpawning = true;
            canDropItem = true;

            // เริ่ม spawn ศัตรูแต่ละกลุ่มพร้อมกัน
            foreach (EnemyGroupData group in enemyGroups)
                StartCoroutine(SpawnEnemyGroup(group));

            // เล่นรอบนี้ตามเวลาที่กำหนด
            yield return new WaitForSeconds(roundDuration);

            // ✅ จบรอบ
            isSpawning = false;
            canDropItem = false;

            // ✅ ลบศัตรูทั้งหมดออก
            ClearAllEnemies();

            Debug.Log($"✅ Round {currentRound} ended! Taking a break...");
            yield return new WaitForSeconds(breakDuration);
        }

        Debug.Log("🏆 All Rounds Complete! You Win!");
        OnGameWin();
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

    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (!enemyPrefab) return;

        Vector3 spawnPos = RandomSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemy.tag = "Enemy";

        // ปรับความยากตามรอบ
        EnemyController e = enemy.GetComponent<EnemyController>();
        if (e != null)
        {
            float diff = Mathf.Pow(difficultyMultiplier, currentRound - 1);
            e.moveSpeed *= diff;
            e.health *= diff;
            e.attackDamage *= diff; // ✅ เพิ่มความแรงโจมตี
        }
    }

    void ClearAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemies)
        {
            Destroy(e);
        }
    }

    Vector3 RandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 pos = new Vector3(randomCircle.x, 0f, randomCircle.y);
        pos += player.position;
        return pos;
    }

    void OnGameWin()
    {
        Debug.Log("🎉 VICTORY! GAME COMPLETE!");
        // ตัวอย่าง: ไปหน้า Victory Scene
        // SceneManager.LoadScene("VictoryScene");
    }
}
