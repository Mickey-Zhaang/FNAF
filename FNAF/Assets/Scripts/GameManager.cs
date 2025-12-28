using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    GameOver,
    NightComplete,
    Paused
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int currentNight = 1;
    public float gameTime = 0f; // Time in seconds (6 AM = 360 seconds = 6 minutes)
    public float nightDuration = 360f; // 6 minutes = 6 AM
    public GameState currentState = GameState.Menu;

    [Header("System References")]
    public PowerSystem powerSystem;
    public CameraSystem cameraSystem;
    public DoorSystem doorSystem;
    public LightSystem lightSystem;
    public AudioManager audioManager;
    public JumpscareSystem jumpscareSystem;

    [Header("UI References")]
    public GameObject mainMenuUI;
    public GameObject gameUI;
    public GameObject gameOverUI;
    public GameObject nightCompleteUI;

    private bool isGameActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeSystems();
        SetGameState(GameState.Menu);
    }

    private void Update()
    {
        if (currentState == GameState.Playing && isGameActive)
        {
            UpdateGameTime();
            CheckWinCondition();
            CheckPowerFailure();
        }

        HandleInput();
    }

    private void InitializeSystems()
    {
        // Get or create system references
        if (powerSystem == null)
            powerSystem = FindObjectOfType<PowerSystem>();
        if (cameraSystem == null)
            cameraSystem = FindObjectOfType<CameraSystem>();
        if (doorSystem == null)
            doorSystem = FindObjectOfType<DoorSystem>();
        if (lightSystem == null)
            lightSystem = FindObjectOfType<LightSystem>();
        if (audioManager == null)
            audioManager = FindObjectOfType<AudioManager>();
        if (jumpscareSystem == null)
            jumpscareSystem = FindObjectOfType<JumpscareSystem>();
    }

    public void StartNight(int nightNumber)
    {
        currentNight = nightNumber;
        gameTime = 0f;
        SetGameState(GameState.Playing);
        isGameActive = true;

        // Initialize systems
        if (powerSystem != null)
            powerSystem.ResetPower();

        if (cameraSystem != null)
            cameraSystem.ResetCameras();

        if (doorSystem != null)
            doorSystem.ResetDoors();

        if (lightSystem != null)
            lightSystem.ResetLights();

        // Initialize animatronics based on night difficulty
        InitializeAnimatronics();

        // Show game UI
        if (gameUI != null)
            gameUI.SetActive(true);
        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);
    }

    public void SetGameState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.Menu:
                Time.timeScale = 1f;
                if (mainMenuUI != null) mainMenuUI.SetActive(true);
                if (gameUI != null) gameUI.SetActive(false);
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                if (mainMenuUI != null) mainMenuUI.SetActive(false);
                if (gameUI != null) gameUI.SetActive(true);
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                isGameActive = false;
                if (gameOverUI != null) gameOverUI.SetActive(true);
                break;
            case GameState.NightComplete:
                Time.timeScale = 0f;
                isGameActive = false;
                if (nightCompleteUI != null) nightCompleteUI.SetActive(true);
                break;
        }
    }

    private void UpdateGameTime()
    {
        gameTime += Time.deltaTime;
    }

    private void CheckWinCondition()
    {
        if (gameTime >= nightDuration)
        {
            CompleteNight();
        }
    }

    private void CheckPowerFailure()
    {
        if (powerSystem != null && powerSystem.GetPowerLevel() <= 0f)
        {
            GameOver("Power Failure");
        }
    }

    public void GameOver(string reason = "Jumpscare")
    {
        if (currentState == GameState.Playing)
        {
            SetGameState(GameState.GameOver);
            if (audioManager != null)
                audioManager.PlayGameOverSound();
        }
    }

    public void CompleteNight()
    {
        if (currentState == GameState.Playing)
        {
            SetGameState(GameState.NightComplete);
            if (audioManager != null)
                audioManager.PlayNightCompleteSound();
        }
    }

    public void TriggerJumpscare(string animatronicName)
    {
        if (jumpscareSystem != null)
        {
            jumpscareSystem.TriggerJumpscare(animatronicName);
        }
        GameOver("Jumpscare");
    }

    private void InitializeAnimatronics()
    {
        // Get all animatronics and set their difficulty based on current night
        AnimatronicBase[] animatronics = FindObjectsOfType<AnimatronicBase>();
        foreach (var animatronic in animatronics)
        {
            animatronic.SetNightDifficulty(currentNight);
            animatronic.ResetPosition();
        }
    }

    private void HandleInput()
    {
        // Pause menu (ESC)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (currentState == GameState.Playing)
            {
                SetGameState(GameState.Paused);
            }
            else if (currentState == GameState.Paused)
            {
                SetGameState(GameState.Playing);
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        SetGameState(GameState.Menu);
        SceneManager.LoadScene(0); // Load main menu scene
    }

    public float GetTimeRemaining()
    {
        return Mathf.Max(0f, nightDuration - gameTime);
    }

    public string GetTimeString()
    {
        float timeRemaining = GetTimeRemaining();
        int hours = Mathf.FloorToInt(timeRemaining / 60f);
        int minutes = Mathf.FloorToInt(timeRemaining % 60f);
        return string.Format("{0:D2}:{1:D2} AM", 12 + hours, minutes);
    }
}

