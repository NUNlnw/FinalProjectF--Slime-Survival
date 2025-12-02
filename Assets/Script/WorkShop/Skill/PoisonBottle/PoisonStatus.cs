using UnityEngine;

public class PoisonStatus : MonoBehaviour
{
    Enemy enemy;

    bool isPoisoned = false;
    float remainingTime = 0f;

    float tickInterval = 0.1f;
    float tickTimer = 0f;
    int damagePerTick = 1;

    void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    // เรียกตอนโดนวงพิษครั้งแรก
    public void ApplyPoison(float duration, int damagePerTick, float tickInterval)
    {
        // ถ้าติดพิษอยู่แล้ว -> ไม่ซ้อน, ไม่รีเฟรช, รอพิษเก่าหมดก่อน
        if (isPoisoned) return;

        this.remainingTime = duration;
        this.damagePerTick = damagePerTick;
        this.tickInterval = tickInterval;
        this.tickTimer = 0f;
        isPoisoned = true;
    }

    void Update()
    {
        if (!isPoisoned) return;

        remainingTime -= Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickInterval)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(damagePerTick);
            }
            tickTimer = 0f;
        }

        if (remainingTime <= 0f)
        {
            isPoisoned = false;
        }
    }
}
