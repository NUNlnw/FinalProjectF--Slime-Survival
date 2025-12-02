using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Character player;   // ลาก Player ที่มี Character ไปใส่
    public Image hpFill;       // รูปแท่งเลือด (Image แบบ Filled)

    void Update()
    {
        if (hpFill == null) return;

        // ถ้า Player ถูกทำลายไปแล้ว -> หลอดต้องเป็น 0
        if (player == null)
        {
            hpFill.fillAmount = 0f;
            return;
        }

        int current = player.health;
        int max = player.maxHealth;

        // ถ้า hp <= 0 -> บังคับ 0 เลย
        if (current <= 0 || max <= 0)
        {
            hpFill.fillAmount = 0f;
            return;
        }

        float fill = (float)current / max;
        hpFill.fillAmount = Mathf.Clamp01(fill);
    }
}

