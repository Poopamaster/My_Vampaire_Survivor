using UnityEngine;
using System.Collections.Generic;

public class CircleSwordManager : MonoBehaviour
{
    public GameObject swordPrefab;
    public int maxSwords = 8;
    public float radius = 2f;
    public float rotateSpeed = 200f;
    public float heightOffset = 1.5f;

    private List<GameObject> swords = new List<GameObject>();

    void Update()
    {
        UpdateSwordPositions();
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

    void OnTriggerEnter(Collider other)
    {
        // ถ้าชนกับ pickup ที่มี tag CircleSword
        if (other.CompareTag("CircleSword"))
        {
            AddSword();
            Destroy(other.gameObject); // ลบ pickup ออกจากฉาก
        }
    }
}
