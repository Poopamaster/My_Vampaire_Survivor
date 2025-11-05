using UnityEngine;

public class CircleSword : MonoBehaviour
{
    private Transform player;
    private float rotateSpeed;
    private float radius;
    private float heightOffset;
    private float baseAngle;

    public float damage = 50f;

    public void SetOrbit(float angle, float radius, float rotateSpeed, float heightOffset)
    {
        this.radius = radius;
        this.rotateSpeed = rotateSpeed;
        this.heightOffset = heightOffset;
        this.baseAngle = angle;

        if (player == null)
            player = transform.parent;
    }

    void Update()
    {
        if (player == null) return;

        float angle = baseAngle + Time.time * rotateSpeed;
        Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
        transform.position = player.position + offset + Vector3.up * heightOffset;

        Vector3 dir = (transform.position - player.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
