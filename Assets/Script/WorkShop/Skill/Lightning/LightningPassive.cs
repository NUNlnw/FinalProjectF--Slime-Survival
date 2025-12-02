using System.Collections.Generic;
using UnityEngine;

public class LightningPassive : PassiveSkill
{
    Transform spawnPoint;          // จุดเกิดลูกบอลสายฟ้า (เช่น ใต้ Player)
    GameObject projectilePrefab;   // Prefab ลูกบอลสายฟ้า

    float radius;                  // ระยะค้นหา enemy
    int projectileCount;           // จำนวนลูกต่อ 1 ครั้งที่ยิง
    int damage;                    // ดาเมจต่อลูก
    float projectileSpeed;         // ความเร็วลูกบอล

    public LightningPassive(
        Transform spawnPoint,
        GameObject projectilePrefab,
        float interval,
        int level,
        float radius,
        int projectileCount,
        int damage,
        float projectileSpeed
    ) : base("Lightning Passive", interval, level)
    {
        this.spawnPoint = spawnPoint;
        this.projectilePrefab = projectilePrefab;
        this.radius = radius;
        this.projectileCount = projectileCount;
        this.damage = damage;
        this.projectileSpeed = projectileSpeed;
    }

    public override void Activate(Character owner)
    {
        if (spawnPoint == null || projectilePrefab == null) return;

        // หา Enemy ในระยะ
        Collider[] hits = Physics.OverlapSphere(spawnPoint.position, radius);
        List<Enemy> enemies = new List<Enemy>();

        foreach (var hit in hits)
        {
            Enemy e = hit.GetComponent<Enemy>();
            if (e != null)
            {
                enemies.Add(e);
            }
        }

        if (enemies.Count == 0) return;

        // สุ่ม +/ หรือเลือกตามระยะก็ได้
        // ที่นี่จะเลือก "ตัวที่ใกล้ที่สุดก่อน แล้วไม่ซ้ำเป้าหมาย"
        int shots = Mathf.Min(projectileCount, enemies.Count);

        for (int i = 0; i < shots; i++)
        {
            Enemy best = null;
            float bestDistSqr = float.MaxValue;

            foreach (var e in enemies)
            {
                float d = (e.transform.position - spawnPoint.position).sqrMagnitude;
                if (d < bestDistSqr)
                {
                    bestDistSqr = d;
                    best = e;
                }
            }

            if (best == null) break;

            // สร้างลูกบอลสายฟ้าเล็งเป้าหมายตัวนี้
            GameObject projObj = Object.Instantiate(
                projectilePrefab,
                spawnPoint.position,
                Quaternion.identity
            );

            LightningProjectile proj = projObj.GetComponent<LightningProjectile>();
            if (proj != null)
            {
                proj.Init(best.transform, damage, projectileSpeed);
            }

            // ลบศัตรูตัวนี้ออกจากลิสต์ เพื่อไม่ให้ลูกต่อไปเล็งซ้ำตัวเดิม
            enemies.Remove(best);
        }
        // เล่นเสียง
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayLightningPassive();
    }
    public bool Upgrade_Count()
    {
        projectileCount += 1;
        return true;
    }

    public bool Upgrade_Damage()
    {
        damage += 2;
        return true;
    }

    public bool Upgrade_Interval()
    {
        if (interval <= 0.2f) return false;
        interval = Mathf.Max(0.2f, interval - 0.2f);
        return true;
    }

    public bool Upgrade_Radius()
    {
        if (radius >= 20f) return false;
        radius = Mathf.Min(20f, radius + 1f);
        return true;
    }

}
