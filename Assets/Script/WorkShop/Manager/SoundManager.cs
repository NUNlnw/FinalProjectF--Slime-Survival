using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Main SFX Source")]
    public AudioSource sfxSource;

    [Header("Footstep SFX")]
    public AudioClip[] footstepClips;
    public float footstepInterval = 0.4f;
    Coroutine footstepRoutine;

    [Header("Exp Pickup SFX")]
    public AudioClip[] expPickupClips;

    [Header("Enemy SFX")]
    public AudioClip[] enemyDeathClips;
    public AudioClip[] enemyHitClips;

    [Header("Level Up SFX")]
    public AudioClip[] levelUpClips;

    [Header("Item SFX")]
    public AudioClip[] healPickupClips;   // ใส่เสียงเก็บ Heal Potion ใน Inspector
    public AudioClip[] speedPickupClips;
    public AudioClip[] bombPickupClips;

    [Header("Passive SFX")]
    public AudioClip[] lightningPassiveClips;
    public AudioClip[] poisonBottleClips;
    public AudioClip[] fireBreathClips;
    public AudioClip[] shockWaveClips;

    [Header("Player SFX")]
    public AudioClip[] playerHitClips;   // << เพิ่มบรรทัดนี้ (ลากเสียงโดนตีของ player มาใส่)

    [Header("Game State SFX")]
    public AudioClip[] playerDeathClips;   // << เสียงตอนผู้เล่นตาย
    public AudioClip[] gameClearClips;     // << เสียงตอนชนะ / หมดเวลา

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // เผื่อเราลืมเซ็ต AudioSource ใน Inspector
        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }


    // ---------- helper: เล่นเสียงจาก array แบบสุ่ม ----------
    void PlayRandomSFX(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0 || sfxSource == null) return;

        int index = Random.Range(0, clips.Length);
        var clip = clips[index];
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // ============ FOOTSTEP LOOP ============

    public void StartFootstepLoop()
    {
        if (footstepRoutine == null && footstepClips != null && footstepClips.Length > 0)
        {
            footstepRoutine = StartCoroutine(FootstepLoop());
        }
    }

    public void StopFootstepLoop()
    {
        if (footstepRoutine != null)
        {
            StopCoroutine(footstepRoutine);
            footstepRoutine = null;
        }
    }

    IEnumerator FootstepLoop()
    {
        while (true)
        {
            PlayRandomSFX(footstepClips);
            yield return new WaitForSeconds(footstepInterval);
        }
    }

    // ============ EXP PICKUP ============

    public void PlayExpPickup()
    {
        PlayRandomSFX(expPickupClips);
    }

    // ============ ENEMY DEATH ============

    public void PlayEnemyDeath()
    {
        PlayRandomSFX(enemyDeathClips);
    }

    public void PlayLevelUp()
    {
        PlayRandomSFX(levelUpClips);
    }
    public void PlayHealPickup()
    {
        PlayRandomSFX(healPickupClips);
    }
    public void PlaySpeedPickup()
    {
        PlayRandomSFX(speedPickupClips);
    }
    public void PlayBombPickup()
    {
        PlayRandomSFX(bombPickupClips);
    }
    public void PlayLightningPassive()
    {
        PlayRandomSFX(lightningPassiveClips);
    }

    public void PlayPoisonBottle()
    {
        PlayRandomSFX(poisonBottleClips);
    }

    public void PlayFireBreath()
    {
        PlayRandomSFX(fireBreathClips);
    }

    public void PlayShockWave()
    {
        PlayRandomSFX(shockWaveClips);
    }
    public void PlayEnemyHit()
    {
        PlayRandomSFX(enemyHitClips);
    }
    public void PlayPlayerHit()          // << เพิ่ม method นี้
    {
        PlayRandomSFX(playerHitClips);
    }
    
    public void PlayPlayerDeath()
    {
        PlayRandomSFX(playerDeathClips);
    }

    public void PlayGameClear()
    {
        PlayRandomSFX(gameClearClips);
    }
}
