using System.Collections.Generic;
using UnityEngine;

public class FireBreathArea : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] int damagePerTick = 1;    // ดาเมจต่อครั้ง
    [SerializeField] float tickInterval = 1f;  // ทุกกี่วิจะโดน 1 ครั้ง

    [Header("Knockback")]
    [SerializeField] float knockbackForce = 3f;

    [Header("Lifetime")]
    [SerializeField] float lifeTime = 3f;

    Character owner;

    // เก็บเวลา "ยิงดาเมจครั้งล่าสุด" ของศัตรูแต่ละตัว
    Dictionary<Enemy, float> lastHitTime = new Dictionary<Enemy, float>();

    public void SetOwner(Character c)
    {
        owner = c;
    }

    void Update()
    {
        // นับเวลาอายุของ VFX
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        // เอาเวลาที่ตีครั้งล่าสุดของ enemy ตัวนี้ (ถ้าไม่มีให้ถือว่า -infinity)
        float lastTime;
        if (!lastHitTime.TryGetValue(enemy, out lastTime))
        {
            lastTime = -9999f;
        }

        // ถ้ายังไม่ถึง interval ก็ยังไม่ดาเมจ
        if (Time.time - lastTime < tickInterval) return;

        // ---------- ทำดาเมจ ----------
        enemy.TakeDamage(damagePerTick);

        // ---------- ผลักออก ----------
        if (owner != null)
        {
            Rigidbody rb = enemy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (enemy.transform.position - owner.transform.position).normalized;
                rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
            }
        }

        // บันทึกเวลาที่โดนครั้งล่าสุด
        lastHitTime[enemy] = Time.time;
    }

    Vector3 baseScale;
    void Awake()
    {
        baseScale = transform.localScale;
    }
    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && lastHitTime.ContainsKey(enemy))
        {
            lastHitTime.Remove(enemy);
        }
    }
    public void Init(Character owner, float dmg, float tickInt, float life, float scale)
    {
        this.owner = owner;
        damagePerTick = Mathf.RoundToInt(dmg);
        tickInterval = tickInt;
        lifeTime = life;
        transform.localScale = baseScale * scale;
    }
}
