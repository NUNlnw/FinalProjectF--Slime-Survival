using UnityEngine;

public class Bomb : Item
{
    public GameObject explosionVfx;   // VFX prefab
    public float radius = 3f;         // damage radius
    public int damage = 20;           // damage to each enemy
    public float knockbackForce = 5f;
    public float vfxLifeTime = 1.5f;

    public override void OnCollect(Player player)
    {
        base.OnCollect(player);

        Vector3 pos = transform.position;

        // spawn VFX
        if (explosionVfx != null)
        {
            GameObject vfx = Instantiate(explosionVfx, pos, Quaternion.identity);
            Destroy(vfx, vfxLifeTime);
        }

        // damage enemies in radius
        Collider[] hits = Physics.OverlapSphere(pos, radius);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                Rigidbody rb = enemy.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 dir = (enemy.transform.position - pos).normalized;
                    rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
                }
            }
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBombPickup();
        }

        // remove the pickup
        Destroy(gameObject);
    }

    // just for visualizing radius in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
