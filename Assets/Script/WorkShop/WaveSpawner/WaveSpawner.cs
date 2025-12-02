using UnityEngine;
using System.Collections;
using System.Collections.Generic; // แม้ว่าจะไม่ได้ใช้ List แต่ควรรวมไว้

public class WaveSpawner : MonoBehaviour
{
    // กำหนดจุดเกิด (Spawn Points) ใน Inspector
    public Transform[] spawnPoints;

    // ข้อมูลสำหรับแต่ละ Wave
    [System.Serializable]
    public class Wave
    {
        public string name;
        public int count; // จำนวนศัตรูใน Wave นี้
        public float rate; // อัตราการเกิด (จำนวนศัตรูต่อวินาที)

        // ** แก้ไข: เพิ่ม Prefab เข้ามาใน Wave Class **
        public GameObject enemyPrefab; // Prefab ศัตรูสำหรับ Wave นี้โดยเฉพาะ
    }

    public Wave[] waves;
    private int nextWave = 0; // ดัชนีของ Wave ถัดไป

    public float timeBetweenWaves = 5f; // เวลาหน่วงก่อนเริ่ม Wave ถัดไป
    private float waveCountdown;

    // ตัวแปรสำหรับตรวจสอบสถานะการเกิดศัตรู
    private bool isSpawning = false;

    private void Start()
    {
        // ตรวจสอบว่ามีจุดเกิดหรือไม่
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("🚨 No spawn points referenced in the Wave Spawner script! Please assign them in the Inspector.");
            // ปิดสคริปต์ถ้าไม่มีจุดเกิด
            enabled = false;
            return;
        }

        waveCountdown = timeBetweenWaves;
    }

    private void Update()
    {
        // ถ้า Wave ยังไม่ครบทั้งหมด และยังไม่ได้อยู่ในกระบวนการ Spawn
        if (nextWave < waves.Length && !isSpawning)
        {
            if (waveCountdown <= 0f)
            {
                // เริ่ม Coroutine สำหรับ Spawn Wave
                StartCoroutine(SpawnWave(waves[nextWave]));
                waveCountdown = timeBetweenWaves; // รีเซ็ตตัวนับ (จะถูกนับอีกครั้งหลัง Wave จบ)
            }
            else
            {
                waveCountdown -= Time.deltaTime;
            }
        }
    }

    // Coroutine สำหรับการจัดการ Wave
    IEnumerator SpawnWave(Wave _wave)
    {
        isSpawning = true; // ตั้งค่าสถานะเป็นกำลังเกิดศัตรู
        Debug.Log("Spawning Wave: " + _wave.name);

        for (int i = 0; i < _wave.count; i++)
        {
            // **✅ แก้ไข: เรียกใช้ Prefab ที่ถูกต้องจาก _wave **
            SpawnEnemy(_wave.enemyPrefab);

            // หน่วงเวลาตามอัตราการเกิด (rate) โดย 1f / rate คือเวลาหน่วงต่อตัว
            yield return new WaitForSeconds(1f / _wave.rate);
        }

        nextWave++; // ไป Wave ถัดไป
        Debug.Log("Wave Completed! Next wave in " + timeBetweenWaves + " seconds.");
        isSpawning = false; // ตั้งค่าสถานะเป็นว่าง เพื่อให้ Wave ถัดไปเริ่มนับถอยหลังได้

        yield break;
    }

    // ฟังก์ชันสำหรับสร้างศัตรูที่จุดเกิดแบบสุ่ม
    void SpawnEnemy(GameObject _enemy)
    {
        // 1. ตรวจสอบ Prefab 
        if (_enemy == null)
        {
            Debug.LogError("Enemy Prefab is null for this wave. Skipping spawn.");
            return;
        }

        // 2. สุ่มเลือกจุดเกิด (Transform) จาก Array
        Transform _sp = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // 3. สร้างศัตรูที่ตำแหน่งและทิศทางของจุดเกิดที่สุ่มมา
        Instantiate(_enemy, _sp.position, _sp.rotation);
    }
}