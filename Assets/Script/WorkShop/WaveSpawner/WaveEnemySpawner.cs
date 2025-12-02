using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEnemySpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;              // จุดเกิดศัตรูแบบสุ่ม

    [Header("Enemy Prefabs")]
    public List<GameObject> normalEnemyPrefabs;  // prefab ศัตรูทั่วไป (สุ่มจากลิสต์)
    public List<GameObject> specialEnemyPrefabs; // prefab ศัตรูพิเศษ (เช่น mini-boss)

    [Header("Wave Settings")]
    public int firstWaveEnemyCount = 5;          // จำนวนตัวใน wave แรก
    public int enemyIncreasePerWave = 2;         // เพิ่มกี่ตัวต่อ wave
    public float timeBetweenSpawns = 0.5f;       // หน่วงเวลาระหว่าง spawn ศัตรูแต่ละตัว
    public int maxAliveEnemies = 20;             // จำนวนศัตรูที่มีอยู่ในฉากพร้อมกันได้สูงสุด

    [Header("Special Enemy Settings")]
    public int specialWaveInterval = 5;          // ทุก ๆ กี่ wave จะเกิด special (0 = ไม่ใช้)

    int currentWave = 0;
    int aliveEnemies = 0;

    void Start()
    {
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        // รอให้ฉากเซ็ตตัวเสร็จนิดนึง
        yield return new WaitForSeconds(1f);

        while (true)
        {
            currentWave++;

            int enemiesThisWave = firstWaveEnemyCount
                                  + enemyIncreasePerWave * (currentWave - 1);

            Debug.Log($"Spawn Wave {currentWave} : {enemiesThisWave} enemies");

            // spawn ศัตรูใน wave นี้
            for (int i = 0; i < enemiesThisWave; i++)
            {
                // รอจนกว่าจำนวนศัตรูในฉากจะยังไม่เกิน maxAliveEnemies
                while (aliveEnemies >= maxAliveEnemies)
                    yield return null;

                SpawnRandomNormalEnemy();
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            // ถ้า wave นี้เป็น wave พิเศษก็ spawn เพิ่ม
            if (specialWaveInterval > 0 &&
                specialEnemyPrefabs.Count > 0 &&
                currentWave % specialWaveInterval == 0)
            {
                SpawnRandomSpecialEnemy();
            }

            // รอจนกว่าศัตรูใน wave นี้จะตายหมดก่อนเริ่ม wave ถัดไป
            yield return new WaitUntil(() => aliveEnemies == 0);

            // พักหายใจนิดหน่อยก่อน wave ถัดไป
            yield return new WaitForSeconds(2f);
        }
    }

    void SpawnRandomNormalEnemy()
    {
        if (normalEnemyPrefabs == null || normalEnemyPrefabs.Count == 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        GameObject prefab = normalEnemyPrefabs[Random.Range(0, normalEnemyPrefabs.Count)];
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        SpawnEnemy(prefab, point.position);
    }

    void SpawnRandomSpecialEnemy()
    {
        if (specialEnemyPrefabs == null || specialEnemyPrefabs.Count == 0) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        GameObject prefab = specialEnemyPrefabs[Random.Range(0, specialEnemyPrefabs.Count)];
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        SpawnEnemy(prefab, point.position);
    }

    void SpawnEnemy(GameObject prefab, Vector3 position)
    {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);

        Enemy enemy = obj.GetComponent<Enemy>();
        if (enemy != null)
        {
            aliveEnemies++;

            // สมัคร event ตายไว้ เพี่อลดจำนวน aliveEnemies เมื่อศัตรูตาย
            enemy.OnDestory += OnEnemyDead;
        }
        else
        {
            Debug.LogWarning("Spawned object has no Enemy component: " + prefab.name);
        }
    }

    void OnEnemyDead(Idestoryable dead)
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);

        Enemy enemy = dead as Enemy;
        if (enemy != null)
        {
            enemy.OnDestory -= OnEnemyDead;
        }
    }
}
