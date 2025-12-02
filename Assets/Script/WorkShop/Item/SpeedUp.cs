using UnityEngine;
using System.Collections;

public class SpeedUp : Item
{
    [Header("Speed Settings")]
    public float speedMultiplier = 1.5f;  // 1.5 = +50% move speed
    public float duration = 5f;           // seconds

    [Header("Pickup VFX")]
    public GameObject pickupVfx;          // VFX prefab
    public float vfxLifeTime = 5f;        // destroy after X seconds
    public string attachPointName = "PassiveSpawner"; // child under Player

    public override void OnCollect(Player player)
    {
        base.OnCollect(player);

        if (player != null)
        {
            // ----- spawn VFX on player -----
            if (pickupVfx != null)
            {
                Transform attach = player.transform.Find(attachPointName);
                if (attach == null) attach = player.transform;

                GameObject vfx = Instantiate(
                    pickupVfx,
                    attach.position,
                    attach.rotation,
                    attach            // parent to player so it follows
                );
                Destroy(vfx, vfxLifeTime);
            }
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySpeedPickup();
            }
            // ----- start speed buff -----
            player.StartCoroutine(SpeedBuffRoutine(player));
        }

        Destroy(gameObject); // remove pickup
    }

    private IEnumerator SpeedBuffRoutine(Player player)
    {
        if (player == null) yield break;

        float originalSpeed = player.movementSpeed;
        player.movementSpeed = originalSpeed * speedMultiplier;

        float t = 0f;
        while (t < duration && player != null)
        {
            t += Time.deltaTime;
            yield return null;
        }

        if (player != null)
        {
            player.movementSpeed = originalSpeed;
        }
    }
}
