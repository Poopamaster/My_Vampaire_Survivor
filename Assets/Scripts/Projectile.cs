using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 5f;
    public AudioClip hitPlayerSound;


    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);

                // 🔊 เสียงตอนผู้เล่นโดนโจมตี
                if (AudioManager.instance != null && hitPlayerSound != null)
                    AudioManager.instance.PlaySound(hitPlayerSound);
            }
        }

        Destroy(gameObject);
    }
}
