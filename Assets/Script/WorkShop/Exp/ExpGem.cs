using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ExpGem : MonoBehaviour
{
    public int expAmount = 10;
    public float magnetRange = 5f;   // ระยะเริ่มดูด
    public float magnetSpeed = 8f;   // ความเร็วตอนวิ่งเข้าหา

    public bool isBeingMagneted = false;

    Transform player;

    void Start()
    {
        // หา player จาก tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
        }
        else
        {
            Debug.LogWarning("ExpGem: ไม่เจอ Player ที่มี Tag = Player");
        }

        // ให้ collider เป็น trigger ไว้
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= magnetRange)
        {
            isBeingMagneted = true;

            Vector3 direction = (player.position - transform.position).normalized;
            float step = magnetSpeed * Time.deltaTime;
            transform.position += direction * step;
        }
        else
        {
            isBeingMagneted = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Experience exp = other.GetComponent<Experience>();
            if (exp != null)
            {
                exp.AddExp(expAmount);
                Debug.Log("Gained " + expAmount + " EXP from gem!");
            }

            // 🔊 เล่นเสียงเก็บ EXP (สุ่มจากลิสต์ใน SoundManager)
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayExpPickup();
            }

            Destroy(gameObject);
        }
    }

    // ให้เห็นระยะดูดใน Scene view เวลาเลือก gem
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, magnetRange);
    }
}
