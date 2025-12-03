using UnityEngine;

public class PoisonBottlePassive : PassiveSkill
{
    // เดิม
    Transform throwPoint;
    GameObject bottlePrefab;
    int bottleCount;
    float minThrowAngleDeg, maxThrowAngleDeg;
    float minThrowSpeed, maxThrowSpeed;
    float spreadAngleDeg;

    // เพิ่มสำหรับระบบพิษ
    int poisonDamagePerTick = 1;
    float poisonTickInterval = 0.1f;
    float poisonScale = 1f;
    float poisonDuration = 1f;
    //float minInterval = 0.2f;

    public PoisonBottlePassive(
        Transform throwPoint,
        GameObject bottlePrefab,
        float interval,
        int level,
        int bottleCount,
        float minThrowAngleDeg,
        float maxThrowAngleDeg,
        float minThrowSpeed,
        float maxThrowSpeed,
        float spreadAngleDeg
    ) : base("Poison Bottle", interval, level)
    {
        this.throwPoint = throwPoint;
        this.bottlePrefab = bottlePrefab;
        this.bottleCount = bottleCount;

        this.minThrowAngleDeg = minThrowAngleDeg;
        this.maxThrowAngleDeg = maxThrowAngleDeg;

        this.minThrowSpeed = minThrowSpeed;
        this.maxThrowSpeed = maxThrowSpeed;

        this.spreadAngleDeg = spreadAngleDeg;
    }

    public override void Activate(Character owner)
    {
        if (throwPoint == null || bottlePrefab == null) return;

        Vector3 baseForward = owner.transform.forward;
        Collider[] ownerCols = owner.GetComponentsInChildren<Collider>();

        for (int i = 0; i < bottleCount; i++)
        {
            GameObject bottleObj = Object.Instantiate(
                bottlePrefab,
                throwPoint.position,
                Quaternion.identity
            );

            PoisonBottleProjectile bottle = bottleObj.GetComponent<PoisonBottleProjectile>();
            if (bottle != null)
            {
                float angle = Random.Range(minThrowAngleDeg, maxThrowAngleDeg);
                float speed = Random.Range(minThrowSpeed, maxThrowSpeed);

                bottle.Launch(
                    baseForward,
                    angle,
                    speed,
                    spreadAngleDeg,
                    ownerCols,
                    poisonDamagePerTick,
                    poisonTickInterval,
                    poisonDuration,
                    poisonScale
                );
            }
        }
        // เล่นเสียง
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayPoisonBottle();

    }

    // ---------- Upgrade methods ----------
    public bool Upgrade_AddBottle()
    {
        bottleCount += 1;
        return true;
    }

    public bool Upgrade_DamagePerTick()
    {
        poisonDamagePerTick += 1;
        return true;
    }

    public bool Upgrade_TickInterval()
    {
        if (poisonTickInterval <= 0.1f) return false;
        poisonTickInterval = Mathf.Max(0.1f, poisonTickInterval - 0.1f);
        return true;
    }

    public bool Upgrade_Scale()
    {
        poisonScale += 0.2f;
        return true;
    }

    public bool Upgrade_Duration()
    {
        poisonDuration += 0.5f;
        return true;
    }

    public bool Upgrade_Interval()
    {
        if (interval <= 0.2f) return false;
        interval = Mathf.Max(0.2f, interval - 0.2f);
        return true;
    }
}
