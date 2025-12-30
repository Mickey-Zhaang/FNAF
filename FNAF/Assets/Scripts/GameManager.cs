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
            powerSystem = FindFirstObjectByType<PowerSystem>();
        if (cameraSystem == null)
            cameraSystem = FindFirstObjectByType<CameraSystem>();
        if (doorSystem == null)
            doorSystem = FindFirstObjectByType<DoorSystem>();
        if (lightSystem == null)
            lightSystem = FindFirstObjectByType<LightSystem>();
        if (audioManager == null)
            audioManager = FindFirstObjectByType<AudioManager>();
        if (jumpscareSystem == null)
            jumpscareSystem = FindFirstObjectByType<JumpscareSystem>();
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
        Debug.LogWarning($"GameOver called with reason: {reason}. Current game state: {currentState}");

        // Allow game over even if not in Playing state (for testing purposes)
        // But prevent multiple game overs
        if (currentState != GameState.GameOver && currentState != GameState.NightComplete)
        {
            Debug.Log($"Setting game state to GameOver. Previous state: {currentState}");
            SetGameState(GameState.GameOver);

            if (audioManager != null)
            {
                audioManager.PlayGameOverSound();
            }
            else
            {
                Debug.LogWarning("AudioManager is NULL! Game over sound may not play.");
            }

            // Also try to show GameUI game over screen if available
            GameUI gameUIComponent = FindFirstObjectByType<GameUI>();
            if (gameUIComponent != null)
            {
                gameUIComponent.ShowGameOver(reason);
            }
            else
            {
                Debug.LogWarning("GameUI component not found. Game over screen may not display.");
            }
        }
        else
        {
            Debug.LogWarning($"GameOver called but game state is already {currentState}. Ignoring.");
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
        AnimatronicBase[] animatronics = FindObjectsByType<AnimatronicBase>(FindObjectsSortMode.None);
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
        // Reset game state first
        SetGameState(GameState.Playing);
        Time.timeScale = 1f;

        // Release all waypoints
        LocationManager locationMgr = FindFirstObjectByType<LocationManager>();
        if (locationMgr != null)
        {
            locationMgr.ReleaseAllWaypoints();
        }

        // Reset all animatronics
        AnimatronicBase[] animatronics = FindObjectsByType<AnimatronicBase>(FindObjectsSortMode.None);
        foreach (var animatronic in animatronics)
        {
            animatronic.ResetPosition();
        }

        // Reset systems
        if (powerSystem != null)
            powerSystem.ResetPower();
        if (cameraSystem != null)
            cameraSystem.ResetCameras();
        if (doorSystem != null)
            doorSystem.ResetDoors();
        if (lightSystem != null)
            lightSystem.ResetLights();

        // Reset game time
        gameTime = 0f;
        isGameActive = true;

        // Reload scene to ensure clean state
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Reset game state without reloading scene (useful for testing)
    /// </summary>
    public void ResetGameState()
    {
        // Reset game state
        SetGameState(GameState.Playing);
        Time.timeScale = 1f;

        // Release all waypoints
        LocationManager locationMgr = FindFirstObjectByType<LocationManager>();
        if (locationMgr != null)
        {
            locationMgr.ReleaseAllWaypoints();
        }

        // Reset all animatronics
        AnimatronicBase[] animatronics = FindObjectsByType<AnimatronicBase>(FindObjectsSortMode.None);
        foreach (var animatronic in animatronics)
        {
            animatronic.ResetPosition();
        }

        // Reset systems
        if (powerSystem != null)
            powerSystem.ResetPower();
        if (cameraSystem != null)
            cameraSystem.ResetCameras();
        if (doorSystem != null)
            doorSystem.ResetDoors();
        if (lightSystem != null)
            lightSystem.ResetLights();

        // Reset game time
        gameTime = 0f;
        isGameActive = true;

        Debug.Log("Game state reset - ready to test again!");
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

