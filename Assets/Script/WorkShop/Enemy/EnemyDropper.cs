using System.Collections.Generic;
using UnityEngine;

public class EnemyDropper : MonoBehaviour
{
    [Header("Always drop")]
    public GameObject expGemPrefab;        // exp ที่ดรอปทุกครั้ง

    [Header("Optional random items")]
    [Range(0f, 1f)]
    public float specialDropChance = 0.05f;  // 5% = 0.05
    public List<GameObject> specialItemPrefabs; // Heal, SpeedUp, Bomb ฯลฯ

    Enemy enemy;

    void Awake()
    {
        enemy = GetComponent<Enemy>();

        if (enemy != null)
        {
            // สมัครกับ event ตายของ Enemy
            enemy.OnDestory += OnEnemyDead;
        }
        else
        {
            Debug.LogWarning("EnemyDropper: Enemy component not found.");
        }
    }

    void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.OnDestory -= OnEnemyDead;
        }
    }

    void OnEnemyDead(Idestoryable dead)
    {
        Vector3 dropPos = transform.position;

        // 1) ดรอป expGem ทุกครั้ง
        if (expGemPrefab != null)
        {
            Instantiate(expGemPrefab, dropPos, Quaternion.identity);
            Debug.Log("Drop ExpGem");
        }

        // 2) เช็กโอกาสดรอป item พิเศษ
        if (specialItemPrefabs != null && specialItemPrefabs.Count > 0)
        {
            if (Random.value <= specialDropChance)
            {
                // สุ่ม item 1 ชิ้นจาก list
                int index = Random.Range(0, specialItemPrefabs.Count);
                GameObject prefab = specialItemPrefabs[index];

                if (prefab != null)
                {
                    Instantiate(prefab, dropPos, Quaternion.identity);
                    Debug.Log("Drop special item: " + prefab.name);
                }
            }
        }
    }
}
