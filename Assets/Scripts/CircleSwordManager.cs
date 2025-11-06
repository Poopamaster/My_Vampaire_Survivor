using UnityEngine;
using System.Collections.Generic;

public class CircleSwordManager : MonoBehaviour
{
    [Header("Sword Settings")]
    public GameObject swordPrefab;
    public int maxSwords = 8;
    public float radius = 2f;
    public float rotateSpeed = 200f;
    public float heightOffset = 1.5f;

    private List<GameObject> swords = new List<GameObject>();

    [Header("Audio Settings")]
    public AudioClip swordSpinSound;  // เสียงหมุนดาบ
    private AudioSource spinAudio;     // ลำโพงกลางของ Manager

    void Start()
    {
        // ✅ สร้าง AudioSource หนึ่งตัวสำหรับเสียงหมุนทั้งหมด
        spinAudio = gameObject.AddComponent<AudioSource>();
        spinAudio.playOnAwake = false;
        spinAudio.loop = true;
        spinAudio.spatialBlend = 0f;
    }

    void Update()
    {
        UpdateSwordPositions();

        // ✅ ถ้ามีดาบ -> เปิดเสียง / ถ้าไม่มี -> ปิดเสียง
        if (swords.Count > 0)
        {
            if (!spinAudio.isPlaying && swordSpinSound != null)
            {
                spinAudio.clip = swordSpinSound;
                spinAudio.Play();
            }
        }
        else
        {
            if (spinAudio.isPlaying)
                spinAudio.Stop();
        }
    }

    void UpdateSwordPositions()
    {
        int count = swords.Count;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            float angle = (360f / count) * i;
            swords[i].GetComponent<CircleSword>().SetOrbit(angle, radius, rotateSpeed, heightOffset);
        }
    }

    public void AddSword()
    {
        if (swords.Count >= maxSwords) return;

        GameObject newSword = Instantiate(swordPrefab, transform.position, Quaternion.identity, transform);
        swords.Add(newSword);
        UpdateSwordPositions();
    }

    public void RemoveAllSwords()
    {
        foreach (var sword in swords)
        {
            if (sword != null)
                Destroy(sword);
        }
        swords.Clear();

        // ✅ หยุดเสียงทันทีเมื่อไม่มีดาบ
        if (spinAudio.isPlaying)
            spinAudio.Stop();
    }

    void OnTriggerEnter(Collider other)
    {
        // 💎 เก็บ Pickup ที่มี Tag "CircleSword"
        if (other.CompareTag("CircleSword"))
        {
            AddSword();
            Destroy(other.gameObject);
        }
    }
}
