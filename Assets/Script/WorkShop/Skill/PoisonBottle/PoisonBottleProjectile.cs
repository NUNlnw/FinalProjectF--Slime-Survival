using UnityEngine;

public class PoisonBottleProjectile : MonoBehaviour
{
    public GameObject poisonAreaPrefab;
    public string groundTag = "Ground";
    public Vector3 poisonAreaRotationEuler;
    public float lifeTime = 5f;

    Rigidbody rb;

    // เก็บค่าพิษที่จะส่งต่อให้ PoisonArea
    int storedDamagePerTick;
    float storedTickInterval;
    float storedPoisonDuration;
    float storedScale = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(
        Vector3 baseForward,
        float throwAngleDeg,
        float throwForce,
        float spreadAngleDeg,
        Collider[] ownerColliders,
        int damagePerTick,
        float tickInterval,
        float poisonDuration,
        float scale
    )
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;

        // --- สุ่มทิศทางในแกน XZ ---
        float yaw = Random.Range(-spreadAngleDeg, spreadAngleDeg);
        Vector3 dirHorizontal = Quaternion.Euler(0f, yaw, 0f) * baseForward;
        dirHorizontal.y = 0f;
        dirHorizontal.Normalize();

        // --- ใส่มุมเงย ---
        float rad = throwAngleDeg * Mathf.Deg2Rad;
        Vector3 dir = dirHorizontal * Mathf.Cos(rad) + Vector3.up * Mathf.Sin(rad);
        dir.Normalize();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(dir * throwForce, ForceMode.Impulse);

        // เก็บค่าไว้ใช้ตอนสร้าง PoisonArea
        storedDamagePerTick = damagePerTick;
        storedTickInterval = tickInterval;
        storedPoisonDuration = poisonDuration;
        storedScale = scale;

        // ไม่ให้ชนกับ owner
        Collider myCol = GetComponent<Collider>();
        if (myCol != null && ownerColliders != null)
        {
            foreach (var c in ownerColliders)
            {
                if (c != null)
                    Physics.IgnoreCollision(myCol, c);
            }
        }
    }

    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(groundTag))
        {
            if (poisonAreaPrefab != null)
            {
                GameObject areaObj = Instantiate(
                    poisonAreaPrefab,
                    transform.position,
                    Quaternion.Euler(poisonAreaRotationEuler)
                );

                PoisonArea area = areaObj.GetComponent<PoisonArea>();
                if (area != null)
                {
                    area.Init(storedDamagePerTick, storedTickInterval, storedPoisonDuration, storedScale);
                }
            }
            Destroy(gameObject);
        }
    }
}
