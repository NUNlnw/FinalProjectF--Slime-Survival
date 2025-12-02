using UnityEngine;

public interface Idestoryable
{
    int health { get; set; }
    int maxHealth { get; set; }

    // ❗ ไม่มี body ตรงนี้เด็ดขาด
    void TakeDamage(int damageAmount);

    event System.Action<Idestoryable> OnDestory;
}
