using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private Text nightText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text powerText;
    [SerializeField] private Image powerBar;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text gameOverText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    [Header("Night Complete UI")]
    [SerializeField] private GameObject nightCompletePanel;
    [SerializeField] private Button nextNightButton;
    [SerializeField] private Button nightCompleteMenuButton;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseMenuButton;

    private GameManager gameManager;
    private PowerSystem powerSystem;
    private AudioManager audioManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        powerSystem = FindObjectOfType<PowerSystem>();
        audioManager = FindObjectOfType<AudioManager>();

        SetupButtons();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (nightCompletePanel != null)
            nightCompletePanel.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void SetupButtons()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMenu);
        if (nextNightButton != null)
            nextNightButton.onClick.AddListener(NextNight);
        if (nightCompleteMenuButton != null)
            nightCompleteMenuButton.onClick.AddListener(ReturnToMenu);
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        if (pauseMenuButton != null)
            pauseMenuButton.onClick.AddListener(ReturnToMenu);
    }

    private void Update()
    {
        UpdateHUD();
        UpdatePauseMenu();
    }

    private void UpdateHUD()
    {
        if (gameManager == null)
            return;

        // Update night text
        if (nightText != null)
        {
            int night = gameManager.currentNight;
            if (night == 6)
                nightText.text = "CUSTOM NIGHT";
            else
                nightText.text = "NIGHT " + night;
        }

        // Update time text
        if (timeText != null)
        {
            timeText.text = gameManager.GetTimeString();
        }

        // Update power
        if (powerSystem != null)
        {
            float power = powerSystem.GetPowerPercentage();
            
            if (powerText != null)
            {
                powerText.text = Mathf.RoundToInt(power) + "%";
            }

            if (powerBar != null)
            {
                powerBar.fillAmount = power / 100f;
            }
        }
    }

    private void UpdatePauseMenu()
    {
        if (gameManager == null || pausePanel == null)
            return;

        if (gameManager.currentState == GameState.Paused && !pausePanel.activeSelf)
        {
            pausePanel.SetActive(true);
        }
        else if (gameManager.currentState != GameState.Paused && pausePanel.activeSelf)
        {
            pausePanel.SetActive(false);
        }
    }

    public void ShowGameOver(string reason)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverText != null)
            {
                gameOverText.text = "GAME OVER\n" + reason.ToUpper();
            }
        }
    }

    public void ShowNightComplete()
    {
        if (nightCompletePanel != null)
        {
            nightCompletePanel.SetActive(true);
        }
    }

    private void RestartGame()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();

        if (gameManager != null)
            gameManager.RestartGame();
    }

    private void ReturnToMenu()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();

        if (gameManager != null)
            gameManager.ReturnToMenu();
    }

    private void NextNight()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();

        if (gameManager != null)
        {
            int nextNight = gameManager.currentNight + 1;
            if (nextNight <= 5)
            {
                gameManager.StartNight(nextNight);
            }
            else
            {
                ReturnToMenu();
            }
        }
    }

    private void ResumeGame()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();

        if (gameManager != null)
            gameManager.SetGameState(GameState.Playing);
    }
}

