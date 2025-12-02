using UnityEngine;

public class ShockWavePassive : PassiveSkill
{
    Transform spawnPoint;
    GameObject vfxPrefab;

    Quaternion rotationOffset;
    float radius;
    int damage;
    float knockbackForce;

    // scale control
    float shockScale = 1f;
    float maxScale = 5f;
    Vector3 baseScale = Vector3.one;

    public ShockWavePassive(
        Transform spawnPoint,
        GameObject vfxPrefab,
        float interval,
        int level,
        Vector3 rotationEuler,
        float radius,
        int damage,
        float knockbackForce
    ) : base("Shock Wave", interval, level)
    {
        this.spawnPoint = spawnPoint;
        this.vfxPrefab = vfxPrefab;
        this.rotationOffset = Quaternion.Euler(rotationEuler);
        this.radius = radius;
        this.damage = damage;
        this.knockbackForce = knockbackForce;

        // remember original prefab scale
        if (vfxPrefab != null)
        {
            baseScale = vfxPrefab.transform.localScale;
        }
    }

    public override void Activate(Character character)
    {
        if (spawnPoint == null || vfxPrefab == null)
        {
            Debug.LogWarning("ShockWavePassive: spawnPoint or vfxPrefab is null");
            return;
        }

        // spawn VFX attached to player
        GameObject vfx = Object.Instantiate(
            vfxPrefab,
            spawnPoint.position,
            spawnPoint.rotation * rotationOffset,
            spawnPoint
        );

        // scale by shockScale
        vfx.transform.localScale = baseScale * shockScale;

        Object.Destroy(vfx, 2f);

        // damage + knockback enemies in radius
        Collider[] hits = Physics.OverlapSphere(spawnPoint.position, radius);
        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                Rigidbody rb = enemy.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 dir = (enemy.transform.position - character.transform.position).normalized;
                    rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
                }
            }
        }
        // เล่นเสียง
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayShockWave();
    }

    // ---------- upgrade functions ----------

    // increase VFX size and radius
    public bool Upgrade_Size()
    {
        if (shockScale >= maxScale) return false;

        shockScale = Mathf.Min(maxScale, shockScale + 0.5f);
        radius += 0.5f;
        return true;
    }

    public bool Upgrade_Damage()
    {
        damage += 3;
        return true;
    }

    public bool Upgrade_Interval()
    {
        if (interval <= 0.5f) return false;
        interval = Mathf.Max(0.5f, interval - 0.2f);
        return true;
    }
}
