using UnityEngine;

public class Enemy : Character
{
    protected enum State { idle, chase, attack, death }

    [SerializeField] float TimeToAttack = 1f;
    protected State currentState = State.idle;
    protected float timer = 0f;

    [Header("Damage Text")]
    public Transform damageTextPoint;        // where the text appears
    public GameObject damageTextPrefab;      // floating text prefab

    [Header("Drop Settings")]
    public GameObject expGemPrefab;          // always dropped

    [Range(0f, 1f)]
    public float extraDropChance = 0.05f;    // chance to drop ONE extra item (5%)

    public GameObject[] extraDropPrefabs;

    [Header("Death VFX")]
    public GameObject deathVfxPrefab;
    public float deathVfxLifetime = 2f;

    bool isDead = false;
    bool hasPlayedDeathSFX = false;

    // ======= NEW: hit sound cooldown =======
    [Header("Hit SFX")]
    [SerializeField] float hitSfxCooldown = 0.1f;   // เวลาขั้นต่ำระหว่างเสียงโดนตี
    float lastHitSfxTime = -999f;                   // เวลาล่าสุดที่เล่นเสียง
    // ----------------- UPDATE / ATTACK -----------------

    protected virtual void Update()
    {
        if (player == null)
        {
            animator.SetBool("Attack", false);
            return;
        }

        Turn(player.transform.position - transform.position);
        timer -= Time.deltaTime;

        if (GetDistanPlayer() < 1.5f)
        {
            Attack(player);
        }
        else
        {
            animator.SetBool("Attack", false);
        }
    }

    protected override void Turn(Vector3 direction)
    {
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;
    }

    protected virtual void Attack(Player target)
    {
        if (timer <= 0)
        {
            target.TakeDamage(Damage);
            animator.SetBool("Attack", true);
            Debug.Log($"{Name} attacks {target.Name} for {Damage} damage.");
            timer = TimeToAttack;
        }
    }

    // ----------------- DAMAGE / DEATH / DROP -----------------

    public override void TakeDamage(int amount)
    {
        int before = health;

        // ให้ Character จัดการลด HP + เช็คตาย
        base.TakeDamage(amount);

        int finalDamage = Mathf.Max(before - health, 0);
        if (finalDamage <= 0) return;
        // ---------- เล่นเสียงโดนตี (ถ้ายังไม่ตาย) ----------
        if (health > 0 && SoundManager.Instance != null)
        {
            if (Time.time - lastHitSfxTime > hitSfxCooldown)
            {
                SoundManager.Instance.PlayEnemyHit();
                lastHitSfxTime = Time.time;
            }
        }

        // สร้างเลขดาเมจ
        if (damageTextPrefab != null && damageTextPoint != null)
        {
            GameObject obj = Instantiate(
                damageTextPrefab,
                damageTextPoint.position,
                Quaternion.identity
            );

            DamageTextFloat dmg = obj.GetComponent<DamageTextFloat>();
            if (dmg == null) dmg = obj.AddComponent<DamageTextFloat>();

            dmg.floatSpeed = 1f;
            dmg.lifeTime = 1f;
            dmg.Init(finalDamage);
        }

        // ---------- ถ้า HP หมด -> นับ kill + ดรอปของ + เล่น VFX ----------
        if (health <= 0 && !isDead)
        {
            isDead = true;

            // นับจำนวนศัตรูที่ฆ่า
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.RegisterEnemyKill();
            }

            // ดรอปของ
            DropItems();

            // สร้าง VFX ตาย
            SpawnDeathVfx();
            // เล่นเสียงตาย (ครั้งเดียว)
            if (!hasPlayedDeathSFX && SoundManager.Instance != null)
            {
                hasPlayedDeathSFX = true;
                SoundManager.Instance.PlayEnemyDeath();
            }
        }

    }

    void SpawnDamageText(int value)
    {
        if (damageTextPrefab == null || damageTextPoint == null) return;

        GameObject obj = Instantiate(
            damageTextPrefab,
            damageTextPoint.position,
            Quaternion.identity
        );

        DamageTextFloat dmg = obj.GetComponent<DamageTextFloat>();
        if (dmg == null) dmg = obj.AddComponent<DamageTextFloat>();

        // default values – you can tune on the prefab instead
        dmg.floatSpeed = 1f;
        dmg.lifeTime = 1f;

        dmg.Init(value);
    }

    void DropItems()
    {
        // always drop EXP gem
        if (expGemPrefab != null)
        {
            Instantiate(expGemPrefab, transform.position, Quaternion.identity);
        }

        // random chance to drop ONE extra item from the list
        if (extraDropPrefabs != null &&
            extraDropPrefabs.Length > 0 &&
            Random.value < extraDropChance)
        {
            // pick a random prefab from the list
            int index = Random.Range(0, extraDropPrefabs.Length);
            GameObject extra = extraDropPrefabs[index];

            if (extra != null)
            {
                Instantiate(extra, transform.position, Quaternion.identity);
            }
        }
    }
    void SpawnDeathVfx()
    {
        if (deathVfxPrefab == null) return;

        GameObject vfx = Instantiate(
            deathVfxPrefab,
            transform.position,       // จุดที่ศัตรูตาย
            Quaternion.identity
        );

        if (deathVfxLifetime > 0f)
        {
            Destroy(vfx, deathVfxLifetime);
        }
    }
}
