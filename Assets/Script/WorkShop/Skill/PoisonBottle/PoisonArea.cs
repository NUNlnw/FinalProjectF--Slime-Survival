using UnityEngine;

public class PoisonArea : MonoBehaviour
{
    [Header("Poison Settings")]
    public float poisonDuration = 1f;       // เวลาติดพิษ
    public int damagePerTick = 1;           // ดาเมจต่อ tick
    public float tickInterval = 0.1f;       // ทุก ๆ กี่วิโดน 1 ดาเมจ

    [Header("Area Lifetime")]
    public float areaLifeTime = 2f;         // วงพิษอยู่บนพื้นนานเท่าไร

    void Update()
    {
        areaLifeTime -= Time.deltaTime;
        if (areaLifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            // หา/เพิ่ม PoisonStatus บน enemy นั้น
            PoisonStatus status = enemy.GetComponent<PoisonStatus>();
            if (status == null)
            {
                status = enemy.gameObject.AddComponent<PoisonStatus>();
            }

            status.ApplyPoison(poisonDuration, damagePerTick, tickInterval);
        }
    }
    public void Init(int dmg, float tickInt, float duration, float scale)
    {
        damagePerTick = dmg;
        tickInterval = tickInt;
        poisonDuration = duration;
        transform.localScale *= scale;  // หรือ baseScale * scale
    }
}
