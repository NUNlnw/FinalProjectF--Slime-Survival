using UnityEngine;

public class LightningProjectile : MonoBehaviour
{
    Transform target;
    int damage;
    float speed;

    public void Init(Transform target, int damage, float speed)
    {
        this.target = target;
        this.damage = damage;
        this.speed = speed;
    }

    void Update()
    {
        if (target == null)
        {
            // ถ้าเป้าหายไปก็ลบตัวเอง
            Destroy(gameObject);
            return;
        }

        // วิ่งเข้าหาเป้าหมาย
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // ให้หันหน้าตามทิศทางที่วิ่ง (ถ้าต้องการ)
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ชน Enemy แล้วทำดาเมจ + ลบตัวเอง
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
