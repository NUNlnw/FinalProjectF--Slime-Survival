using UnityEngine;

public class HealPotion : Item
{
    [Header("Heal Settings")]
    public int healAmount = 20;   // how much HP to restore

    [Header("Pickup VFX")]
    public GameObject pickupVfx;  // ใส่ VFX prefab ที่ Inspector
    public float vfxLifeTime = 1.5f;

    public override void OnCollect(Player player)
    {
        base.OnCollect(player);

        // สร้าง VFX ตอนเก็บ
        if (pickupVfx != null)
        {
            GameObject vfx = Instantiate(pickupVfx, transform.position, Quaternion.identity);
            Destroy(vfx, vfxLifeTime);
        }

        if (player != null)
        {
            player.Heal(healAmount);   // uses Character.Heal()
        }

        // เล่นเสียงเก็บ Heal
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayHealPickup();
        }

        Destroy(gameObject); // ลบตัวไอเท็ม
    }
}
