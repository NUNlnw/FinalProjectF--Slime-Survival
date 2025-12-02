using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("End / Dead UI Panels")]
    public GameObject deadPanel;      // shown when player HP = 0
    public GameObject timeUpPanel;    // shown when time is over

    [Header("Pause UI")]
    public GameObject pausePanel;     // panel for Pause menu
    public Button pauseRestartButton; // restart button on pause UI
    public Button pauseResumeButton;  // resume button on pause UI

    [Header("Common Restart Buttons")]
    public Button deadRestartButton;     // restart button on Dead UI
    public Button timeUpRestartButton;   // restart button on TimeUp UI

    [Header("Timer UI")]
    public TMP_Text timerText;           // TextMeshPro for time
    public float gameDurationSeconds = 600f; // 10 minutes by default

    [Header("Refs")]
    public Player player;

    float timeLeft;
    bool isGameEnded = false;
    bool isPaused = false;

    void Awake()
    {
        // Singleton so Enemy can call GameUIManager.Instance
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Hide all panels at start
        if (deadPanel != null) deadPanel.SetActive(false);
        if (timeUpPanel != null) timeUpPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        // Hook buttons
        if (deadRestartButton != null) deadRestartButton.onClick.AddListener(RestartGame);
        if (timeUpRestartButton != null) timeUpRestartButton.onClick.AddListener(RestartGame);

        if (pauseRestartButton != null) pauseRestartButton.onClick.AddListener(RestartGame);
        if (pauseResumeButton != null) pauseResumeButton.onClick.AddListener(ResumeGame);
    }

    void Start()
    {
        timeLeft = gameDurationSeconds;

        if (player == null) player = Object.FindFirstObjectByType<Player>();

        if (player != null)
        {
            // subscribe to death event (Character.OnDestory)
            player.OnDestory += OnPlayerDead;
        }
        else
        {
            Debug.LogWarning("GameUIManager: Player reference is missing");
        }
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.OnDestory -= OnPlayerDead;
        }

        if (Instance == this) Instance = null;
    }

    void Update()
    {
        // Pause / Resume with ESC (only if game not ended)
        if (!isGameEnded && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }

        // Update timer only if not paused & not ended
        if (!isGameEnded && !isPaused)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0f) timeLeft = 0f;

            UpdateTimerText();

            if (timeLeft <= 0f)
            {
                OnTimeUp();
            }
        }
    }

    void UpdateTimerText()
    {
        if (timerText == null) return;

        int totalSeconds = Mathf.CeilToInt(timeLeft);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // ------------- End conditions -------------

    void OnPlayerDead(Idestoryable deadThing)
    {
        if (isGameEnded) return;

        isGameEnded = true;
        Time.timeScale = 0f;

        // เล่นเสียงตาย
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPlayerDeath();
        }

        if (deadPanel != null) deadPanel.SetActive(true);
    }

    void OnTimeUp()
    {
        if (isGameEnded) return;

        isGameEnded = true;
        Time.timeScale = 0f;

        // เล่นเสียงเคลียร์เกม / หมดเวลา
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayGameClear();
        }

        if (timeUpPanel != null) timeUpPanel.SetActive(true);
    }

    // ------------- Pause -------------

    void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    // ------------- Restart -------------

    public void RestartGame()
    {
        Time.timeScale = 1f;

        // ทำลาย SoundManager ตัวเก่าก่อนโหลดฉากใหม่
        if (SoundManager.Instance != null)
        {
            Destroy(SoundManager.Instance.gameObject);
        }

        var current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }


    // ------------- Enemy kill count (part 2) -------------

    [Header("Kill Counter")]
    public TMP_Text killCountText;
    int killCount = 0;

    public void RegisterEnemyKill()
    {
        killCount++;
        if (killCountText != null)
        {
            killCountText.text = $"KILLS : {killCount}";
        }
    }
}
