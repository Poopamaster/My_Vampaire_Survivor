using UnityEngine;
using System.Collections.Generic;

public class WaveItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ItemSpawnData
    {
        public GameObject itemPrefab;
        [Range(0f, 100f)] public float dropChance = 10f; // % ดรอปเมื่อศัตรูตาย
    }

    [Header("Item Drop Settings")]
    public List<ItemSpawnData> itemsToSpawn;
    public LayerMask groundMask;
    public float itemHeight = 0.3f;

    [Header("Guaranteed Drop (1 per Wave)")]
    [Tooltip("ของที่ดรอปแน่นอน 1 ชิ้นต่อเวฟ")]
    public GameObject guaranteedItemPrefab;

    [Header("Wave Sync")]
    public EnemyWaveSpawner waveSpawner;
    [Tooltip("จำนวนของดรอปสูงสุดต่อ 1 เวฟ (รวมของสุ่มทั้งหมด)")]
    public int maxItemsPerWave = 2;

    private int itemsDroppedThisWave = 0;
    private int lastSeenRound = 0;
    private bool guaranteedItemDropped = false;

    public static WaveItemSpawner Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (waveSpawner == null)
            waveSpawner = FindObjectOfType<EnemyWaveSpawner>();

        if (waveSpawner != null)
            lastSeenRound = Mathf.Max(0, waveSpawner.currentRound);
    }

    public void TrySpawnItem(Vector3 deathPosition)
    {
        AutoResetIfNewRound();

        if (itemsToSpawn == null || itemsToSpawn.Count == 0)
            return;

        // ✅ ถ้ายังไม่ได้ดรอปของแน่นอน ให้ดรอปตอนนี้เลย
        if (!guaranteedItemDropped && guaranteedItemPrefab != null)
        {
            Vector3 spawnPos = GroundedPosition(deathPosition);
            Instantiate(guaranteedItemPrefab, spawnPos, Quaternion.identity);

            guaranteedItemDropped = true;
            itemsDroppedThisWave++;
            return;
        }

        // ✅ ดรอปของสุ่มตามโอกาส
        if (itemsDroppedThisWave >= maxItemsPerWave)
            return;

        foreach (var item in itemsToSpawn)
        {
            float roll = Random.Range(0f, 100f);
            if (roll <= item.dropChance)
            {
                Vector3 spawnPos = GroundedPosition(deathPosition);
                Instantiate(item.itemPrefab, spawnPos, Quaternion.identity);

                itemsDroppedThisWave++;
                break;
            }
        }
    }

    public void ResetDropsForNewWave(int roundIndex)
    {
        lastSeenRound = roundIndex;
        itemsDroppedThisWave = 0;
        guaranteedItemDropped = false; // ✅ รีเซ็ตให้ดรอปของแน่นอนได้ใหม่
    }

    private void AutoResetIfNewRound()
    {
        if (waveSpawner == null) return;
        if (waveSpawner.currentRound != lastSeenRound)
        {
            lastSeenRound = waveSpawner.currentRound;
            itemsDroppedThisWave = 0;
            guaranteedItemDropped = false;
        }
    }

    private Vector3 GroundedPosition(Vector3 near)
    {
        Vector3 pos = near + Vector3.up * 5f;
        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 20f, groundMask))
            return hit.point + Vector3.up * itemHeight;

        // fallback
        return near + Vector3.up * itemHeight;
    }
}
