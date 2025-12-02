using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelUpUIManager : MonoBehaviour
{
    [Header("UI Root")]
    public GameObject levelUpPanel;

    [Header("Option 1")]
    public Button passiveButton1;
    public TMP_Text passiveNameText1;
    public TMP_Text passiveInfoText1;

    [Header("Option 2")]
    public Button passiveButton2;
    public TMP_Text passiveNameText2;
    public TMP_Text passiveInfoText2;

    [Header("Refs")]
    public PassiveSkillBook passiveBook;
    public Experience experience;

    int pendingLevelUps = 0;   // เลเวลที่ต้องเลือกของรางวัล
    PassiveSkillBook.UpgradeOption option1;
    PassiveSkillBook.UpgradeOption option2;

    void Awake()
    {
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);

        if (passiveButton1 != null)
            passiveButton1.onClick.AddListener(() => OnChoose(0));

        if (passiveButton2 != null)
            passiveButton2.onClick.AddListener(() => OnChoose(1));
    }

    // ถูกเรียกจาก Experience.LevelUp()
    public void QueueLevelUp()
    {
        pendingLevelUps++;

        if (!levelUpPanel.activeSelf)
        {
            OpenPanel();
        }
    }

    void OpenPanel()
    {
        Time.timeScale = 0f;   // หยุดเกม
        RefreshOptions();
        levelUpPanel.SetActive(true);
    }

    void ClosePanel()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;   // เล่นต่อ
    }

    void RefreshOptions()
    {
        if (passiveBook == null)
        {
            Debug.LogWarning("LevelUpUIManager: PassiveBook is null");
            return;
        }

        var pair = passiveBook.GetRandomTwoOptions();
        option1 = pair[0];
        option2 = pair[1];

        if (passiveNameText1 != null) passiveNameText1.text = option1.title;
        if (passiveInfoText1 != null) passiveInfoText1.text = option1.description;

        if (passiveNameText2 != null) passiveNameText2.text = option2.title;
        if (passiveInfoText2 != null) passiveInfoText2.text = option2.description;
    }

    void OnChoose(int index)
    {
        if (index == 0 && option1.apply != null)
        {
            option1.apply();
        }
        else if (index == 1 && option2.apply != null)
        {
            option2.apply();
        }

        pendingLevelUps--;

        if (pendingLevelUps > 0)
        {
            // ถ้ายังมีเลเวลค้างอยู่ ให้สุ่มตัวเลือกใหม่ แต่ไม่ปิด UI
            RefreshOptions();
        }
        else
        {
            ClosePanel();
        }
    }
}
