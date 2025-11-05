using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FloatingItem : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatAmplitude = 0.25f;  // ระยะการลอยขึ้นลง
    public float floatFrequency = 2f;     // ความเร็วการลอย
    public float rotateSpeed = 50f;       // ความเร็วการหมุน

    [Header("Lifetime Settings")]
    public float lifetime = 15f;          // เวลาที่อยู่ในฉากก่อนหาย (วินาที)

    [Header("Visual Settings")]
    public Color auraColor = new Color(0.3f, 0.8f, 1f, 0.6f); // สีออร่า
    public float auraIntensity = 2f;
    public float auraRange = 3f;

    private Vector3 startPos;
    private Light auraLight;
    private float lifeTimer;

    void Start()
    {
        startPos = transform.position;
        lifeTimer = lifetime;

        // ✅ ทำให้ Collider เป็น Trigger เพื่อไม่ชนกับ Enemy/Player
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // ✅ เพิ่มออร่ารอบตัว (Light)
        auraLight = gameObject.AddComponent<Light>();
        auraLight.type = LightType.Point;
        auraLight.color = auraColor;
        auraLight.intensity = auraIntensity;
        auraLight.range = auraRange;
    }

    void Update()
    {
        // ✅ ลอยขึ้นลง
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // ✅ หมุนเบา ๆ
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);

        // ✅ ตัวนับเวลาหาย
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    // ✅ กัน Enemy ไม่ชนจนขยับ (เพราะเป็น Trigger แล้ว แต่เผื่อไว้)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }
}
