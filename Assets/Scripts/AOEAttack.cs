using UnityEngine;
using System.Collections;

public class AOEAttack : MonoBehaviour
{
    [Header("AOE Settings")]
    public float damage = 30f;
    public float radius = 3f;
    public float delayBeforeExplode = 1f;   // เวลารอก่อนระเบิด
    public float explosionDuration = 1f;    // เอฟเฟกต์ระเบิดอยู่นานแค่ไหน
    public AudioClip explosionSound;
    public ParticleSystem warningEffect;    // เอฟเฟกต์วงแดงเตือนก่อนระเบิด
    public ParticleSystem explosionEffect;  // เอฟเฟกต์ระเบิดจริง

    private bool exploded = false;

    void Start()
    {
        // ✅ เล่นเอฟเฟกต์เตือน (วงแดง)
        if (warningEffect != null)
            warningEffect.Play();

        StartCoroutine(ExplodeAfterDelay());
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeExplode);

        exploded = true;

        if (warningEffect != null)
            warningEffect.Stop();
        if (explosionEffect != null)
            explosionEffect.Play();

        if (AudioManager.instance != null && explosionSound != null)
            AudioManager.instance.PlaySound(explosionSound);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth player = hit.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    Debug.Log("🔥 Player โดน AOE Damage!");
                    player.TakeDamage(damage);
                }
            }
        }

        // ✅ รอจนเอฟเฟกต์ระเบิดเล่นจบจริง
        float waitTime = explosionEffect != null ? explosionEffect.main.duration : explosionDuration;
        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
