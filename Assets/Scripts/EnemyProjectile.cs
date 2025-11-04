using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float lifeTime = 5f;
    public float damage = 10f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by projectile!");
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
