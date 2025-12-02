using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SceneButton : MonoBehaviour
{
    // ชื่อ scene ที่จะโหลด (เช่น "Map")
    [SerializeField] string sceneName = "Map";

    void Awake()
    {
        // ผูกปุ่มให้เรียกฟังก์ชัน LoadScene อัตโนมัติ
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(LoadScene);
    }

    public void LoadScene()
    {
        Time.timeScale = 1f;                // กันเผื่อมาจากฉากที่ pause
        SceneManager.LoadScene(sceneName);  // โหลดฉากตามชื่อ
    }
}
