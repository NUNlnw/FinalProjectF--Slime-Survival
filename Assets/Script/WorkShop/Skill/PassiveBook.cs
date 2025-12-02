using System.Collections.Generic;
using UnityEngine;

public class PassiveSkillBook : MonoBehaviour
{
    List<PassiveSkill> passiveSkills = new List<PassiveSkill>();
    Character owner;

    [Header("ShockWave Passive Settings")]
    public GameObject shockWavePrefab;
    public Transform shockWaveSpawnPoint;
    public float shockWaveInterval = 2f;
    public int shockWaveLevel = 1;
    public Vector3 shockWaveRotationEuler;
    public float shockWaveRadius = 3f;
    public int shockWaveDamage = 10;
    public float shockWaveKnockback = 5f;

    [Header("Lightning Passive Settings")]
    public GameObject lightningProjectilePrefab;
    public Transform lightningSpawnPoint;
    public float lightningInterval = 1.5f;
    public int lightningLevel = 1;
    public float lightningRadius = 8f;
    public int lightningProjectileCount = 1;
    public int lightningDamage = 10;
    public float lightningProjectileSpeed = 10f;

    [Header("Fire Breath Passive")]
    public GameObject fireBreathPrefab;
    public Transform fireBreathPoint;
    public float fireBreathInterval = 4f;

    [Header("Poison Bottle Passive")]
    public GameObject poisonBottlePrefab;
    public Transform poisonThrowPoint;
    public float poisonBottleInterval = 3f;
    public int poisonBottleCount = 1;

    public float poisonThrowAngleMin = 50f;
    public float poisonThrowAngleMax = 70f;

    public float poisonThrowSpeedMin = 8f;
    public float poisonThrowSpeedMax = 14f;

    public float poisonThrowSpreadAngle = 45f;

    // references to passives (null = locked)
    public PoisonBottlePassive poison { get; private set; }
    public FireBreathPassive fire { get; private set; }
    public LightningPassive lightning { get; private set; }
    public ShockWavePassive shock { get; private set; }

    void Start()
    {
        owner = GetComponent<Character>();
        if (owner == null)
        {
            Debug.LogError("PassiveSkillBook: No Character on this object");
            enabled = false;
            return;
        }

        passiveSkills = new List<PassiveSkill>();

        //------------------------------
        // START GAME: only Lightning is unlocked
        //------------------------------
        if (lightningProjectilePrefab != null)
        {
            lightning = new LightningPassive(
                lightningSpawnPoint != null ? lightningSpawnPoint : owner.transform,
                lightningProjectilePrefab,
                lightningInterval,
                lightningLevel,
                lightningRadius,
                lightningProjectileCount,
                lightningDamage,
                lightningProjectileSpeed
            );
            passiveSkills.Add(lightning);
        }

        // others are locked at start (null)
        shock = null;
        fire = null;
        poison = null;
    }

    void Update()
    {
        if (owner == null) return;

        float dt = Time.deltaTime;
        foreach (var p in passiveSkills)
        {
            p.Tick(owner, dt);
        }
    }

    // central apply function (you already had this)
    public bool ApplyUpgrade(PassiveUpgradeType type)
    {
        switch (type)
        {
            // ----- Poison Bottle -----
            case PassiveUpgradeType.Poison_AddBottle:
                return poison != null && poison.Upgrade_AddBottle();
            case PassiveUpgradeType.Poison_DamagePerTick:
                return poison != null && poison.Upgrade_DamagePerTick();
            case PassiveUpgradeType.Poison_TickInterval:
                return poison != null && poison.Upgrade_TickInterval();
            case PassiveUpgradeType.Poison_Scale:
                return poison != null && poison.Upgrade_Scale();
            case PassiveUpgradeType.Poison_Duration:
                return poison != null && poison.Upgrade_Duration();
            case PassiveUpgradeType.Poison_Interval:
                return poison != null && poison.Upgrade_Interval();

            // ----- Fire Breath -----
            case PassiveUpgradeType.Fire_Scale:
                return fire != null && fire.Upgrade_Scale();
            case PassiveUpgradeType.Fire_Damage:
                return fire != null && fire.Upgrade_Damage();
            case PassiveUpgradeType.Fire_LifeTime:
                return fire != null && fire.Upgrade_LifeTime();

            // ----- Lightning -----
            case PassiveUpgradeType.Lightning_Count:
                return lightning != null && lightning.Upgrade_Count();
            case PassiveUpgradeType.Lightning_Damage:
                return lightning != null && lightning.Upgrade_Damage();
            case PassiveUpgradeType.Lightning_Interval:
                return lightning != null && lightning.Upgrade_Interval();
            case PassiveUpgradeType.Lightning_Radius:
                return lightning != null && lightning.Upgrade_Radius();

            // ----- Shock Wave -----
            case PassiveUpgradeType.Shock_Size:
                return shock != null && shock.Upgrade_Size();
            case PassiveUpgradeType.Shock_Damage:
                return shock != null && shock.Upgrade_Damage();
            case PassiveUpgradeType.Shock_Interval:
                return shock != null && shock.Upgrade_Interval();
        }

        return false;
    }

    // what UI needs
    public struct UpgradeOption
    {
        public string title;       // card title
        public string description; // description text
        public System.Action apply;
    }

    // --------------------------------------------------
    // RANDOM TWO OPTIONS (English text, unlock + upgrade)
    // --------------------------------------------------
    public UpgradeOption[] GetRandomTwoOptions()
    {
        List<UpgradeOption> options = new List<UpgradeOption>();

        // -------- Lightning (always owned) --------
        if (lightning != null)
        {
            options.Add(new UpgradeOption
            {
                title = "Lightning\n+Projectile",
                description = "+1 lightning ball",
                apply = () => ApplyUpgrade(PassiveUpgradeType.Lightning_Count)
            });

            options.Add(new UpgradeOption
            {
                title = "Lightning\n+Damage",
                description = "+2 lightning damage",
                apply = () => ApplyUpgrade(PassiveUpgradeType.Lightning_Damage)
            });
        }

        // -------- Unlock / upgrade Poison Bottle --------
        if (poison == null && poisonBottlePrefab != null)
        {
            // unlock
            options.Add(new UpgradeOption
            {
                title = "Unlock\nPoison Bottle",
                description = "Gain a poison bottle passive",
                apply = () =>
                {
                    poison = new PoisonBottlePassive(
                        poisonThrowPoint != null ? poisonThrowPoint : owner.transform,
                        poisonBottlePrefab,
                        poisonBottleInterval,
                        1,
                        poisonBottleCount,
                        poisonThrowAngleMin,
                        poisonThrowAngleMax,
                        poisonThrowSpeedMin,
                        poisonThrowSpeedMax,
                        poisonThrowSpreadAngle
                    );
                    passiveSkills.Add(poison);
                }
            });
        }
        else if (poison != null)
        {
            options.Add(new UpgradeOption
            {
                title = "Poison Bottle\n+Bottle",
                description = "+1 poison bottle",
                apply = () => ApplyUpgrade(PassiveUpgradeType.Poison_AddBottle)
            });
        }

        // -------- Unlock / upgrade Fire Breath --------
        if (fire == null && fireBreathPrefab != null)
        {
            options.Add(new UpgradeOption
            {
                title = "Unlock\nFire Breath",
                description = "Gain a fire breath passive",
                apply = () =>
                {
                    fire = new FireBreathPassive(
                        fireBreathPoint != null ? fireBreathPoint : owner.transform,
                        fireBreathPrefab,
                        fireBreathInterval
                    );
                    passiveSkills.Add(fire);
                }
            });
        }
        else if (fire != null)
        {
            options.Add(new UpgradeOption
            {
                title = "Fire Breath\n+Damage",
                description = "+0.5 damage per tick",
                apply = () => ApplyUpgrade(PassiveUpgradeType.Fire_Damage)
            });
        }

        // -------- Unlock / upgrade Shock Wave --------
        if (shock == null && shockWavePrefab != null)
        {
            options.Add(new UpgradeOption
            {
                title = "Unlock\nShock Wave",
                description = "Gain a shock wave passive",
                apply = () =>
                {
                    shock = new ShockWavePassive(
                        shockWaveSpawnPoint != null ? shockWaveSpawnPoint : owner.transform,
                        shockWavePrefab,
                        shockWaveInterval,
                        shockWaveLevel,
                        shockWaveRotationEuler,
                        shockWaveRadius,
                        shockWaveDamage,
                        shockWaveKnockback
                    );
                    passiveSkills.Add(shock);
                }
            });
        }
        else if (shock != null)
        {
            options.Add(new UpgradeOption
            {
                title = "Shock Wave\n+Damage",
                description = "+3 shock damage",
                apply = () => ApplyUpgrade(PassiveUpgradeType.Shock_Damage)
            });
        }

        // fallback: if somehow list is empty, always offer lightning damage twice
        if (options.Count == 0)
        {
            options.Add(new UpgradeOption
            {
                title = "Lightning\n+Damage",
                description = "+2 lightning damage",
                apply = () => ApplyUpgrade(PassiveUpgradeType.Lightning_Damage)
            });
        }

        // pick 2 distinct options
        UpgradeOption[] result = new UpgradeOption[2];

        int firstIndex = Random.Range(0, options.Count);
        result[0] = options[firstIndex];
        options.RemoveAt(firstIndex);

        if (options.Count > 0)
        {
            int secondIndex = Random.Range(0, options.Count);
            result[1] = options[secondIndex];
        }
        else
        {
            // only 1 option -> duplicate (or you can handle differently)
            result[1] = result[0];
        }

        return result;
    }
}
