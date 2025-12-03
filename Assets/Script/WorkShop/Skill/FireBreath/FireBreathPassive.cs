using UnityEngine;

public class FireBreathPassive : PassiveSkill
{
    Transform firePoint;         // จุดปล่อยไฟ (เช่น empty ที่ปาก)
    GameObject fireBreathPrefab; // Prefab vfx

    float fireScale = 1f;
    float fireDamagePerTick = 1f;
    float fireTickInterval = 1f; // ถ้ามี
    float fireLifeTime = 3f;
    float maxScale = 3f;

    public FireBreathPassive(
        Transform firePoint,
        GameObject fireBreathPrefab,
        float interval,
        int level = 1
    ) : base("Fire Breath", interval, level)
    {
        this.firePoint = firePoint;
        this.fireBreathPrefab = fireBreathPrefab;
    }

    public override void Activate(Character owner)
    {
        if (firePoint == null || fireBreathPrefab == null) return;

        GameObject obj = Object.Instantiate(
            fireBreathPrefab,
            firePoint.position,
            firePoint.rotation,
            firePoint
        );

        FireBreathArea area = obj.GetComponent<FireBreathArea>();
        if (area != null)
        {
            area.Init(owner, fireDamagePerTick, fireTickInterval, fireLifeTime, fireScale);
        }
        // เล่นเสียง
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayFireBreath();
    }
    public bool Upgrade_Scale()
    {
        if (fireScale >= maxScale) return false;
        fireScale = Mathf.Min(maxScale, fireScale + 0.2f);
        return true;
    }

    public bool Upgrade_Damage()
    {
        fireDamagePerTick += 1f;
        return true;
    }

    public bool Upgrade_LifeTime()
    {
        fireLifeTime += 1f;
        return true;
    }

}
