using UnityEngine;

public abstract class PassiveSkill
{
    public string skillName;
    public int level;          // เลเวลของ passive
    public float interval;     // เกิดทุก ๆ กี่วิ
    protected float timer;     // ตัวจับเวลา

    protected PassiveSkill(string name, float interval, int level = 1)
    {
        this.skillName = name;
        this.interval = interval;
        this.level = level;
        this.timer = interval; // เริ่มต้นให้ครบ interval ก่อนจะยิงครั้งแรก
    }

    // ให้ PassiveBook เรียกทุกเฟรม
    public void Tick(Character character, float deltaTime)
    {
        timer -= deltaTime;
        if (timer <= 0f)
        {
            Activate(character);  // เรียกการทำงานจริงของ passive
            timer = interval;     // รีเซ็ตเวลาใหม่
        }
    }

    // สิ่งที่ passive ทำจริง ๆ
    public abstract void Activate(Character character);
}
