using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ItemSpawnData
    {
        public GameObject itemPrefab;
        [Range(0f, 100f)] public float dropChance = 10f; // ✅ เปลี่ยนเป็นเปอร์เซ็นต์ดรอป
    }

    [Header("Item Drop Settings")]
    public List<ItemSpawnData> itemsToSpawn;
    public LayerMask groundMask;
    public float itemHeight = 0.5f;

    [Header("Wave Sync (Optional)")]
    public EnemyWaveSpawner waveSpawner;

    public static WaveItemSpawner Instance;

    void Awake()
    {
        Instance = this;
    }

    public void TrySpawnItem(Vector3 deathPosition)
    {
        if (itemsToSpawn == null || itemsToSpawn.Count == 0)
            return;

        foreach (var item in itemsToSpawn)
        {
            float roll = Random.Range(0f, 100f);
            if (roll <= item.dropChance)
            {
                Vector3 spawnPos = GetGroundPosition(deathPosition);
                Instantiate(item.itemPrefab, spawnPos, Quaternion.identity);
                Debug.Log($"💎 Dropped: {item.itemPrefab.name} ({item.dropChance}%)");
                break; // ดรอปได้แค่ 1 อย่างต่อศัตรู
            }
        }
    }

    Vector3 GetGroundPosition(Vector3 origin)
    {
        Vector3 pos = origin + Vector3.up * 5f;
        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 10f, groundMask))
        {
            pos = hit.point + Vector3.up * itemHeight;
        }
        return pos;
    }
}
