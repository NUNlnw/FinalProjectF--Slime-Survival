using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Experience : MonoBehaviour
{
    [Header("EXP / Level")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    [Header("UI")]
    public TMP_Text levelText;
    public Image expBarFill;

    [Header("Level Up UI")]
    public LevelUpUIManager levelUpUI;   // <- เพิ่มบรรทัดนี้

    void Start()
    {
        UpdateUI();
    }

    public void AddExp(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }

        UpdateUI();
    }

    void LevelUp()
    {
        level++;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.2f);
        Debug.Log("LEVEL UP! " + level);

        // แจ้ง UI ว่าเลเวลอัพ 1 ครั้ง
        if (levelUpUI != null)
        {
            levelUpUI.QueueLevelUp();
        }
        // เล่นเสียงเลเวลอัพ
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayLevelUp();
        }
    }

    void UpdateUI()
    {
        if (levelText != null)
        {
            levelText.text = "LEVEL : " + level;
        }

        if (expBarFill != null)
        {
            float fill = (float)currentExp / expToNextLevel;
            expBarFill.fillAmount = fill;
        }
    }
}
