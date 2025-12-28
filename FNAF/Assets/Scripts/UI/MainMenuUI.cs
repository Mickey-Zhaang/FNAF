using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button night1Button;
    [SerializeField] private Button night2Button;
    [SerializeField] private Button night3Button;
    [SerializeField] private Button night4Button;
    [SerializeField] private Button night5Button;
    [SerializeField] private Button customNightButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Custom Night UI")]
    [SerializeField] private GameObject customNightPanel;
    [SerializeField] private Slider bonnieAISlider;
    [SerializeField] private Slider chicaAISlider;
    [SerializeField] private Slider freddyAISlider;
    [SerializeField] private Slider foxyAISlider;
    [SerializeField] private Button startCustomNightButton;
    [SerializeField] private Button backButton;

    [Header("Settings UI")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider ambientVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Button settingsBackButton;

    private GameManager gameManager;
    private AudioManager audioManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();

        SetupButtons();
        InitializeSettings();
    }

    private void SetupButtons()
    {
        if (night1Button != null)
            night1Button.onClick.AddListener(() => StartNight(1));
        if (night2Button != null)
            night2Button.onClick.AddListener(() => StartNight(2));
        if (night3Button != null)
            night3Button.onClick.AddListener(() => StartNight(3));
        if (night4Button != null)
            night4Button.onClick.AddListener(() => StartNight(4));
        if (night5Button != null)
            night5Button.onClick.AddListener(() => StartNight(5));
        if (customNightButton != null)
            customNightButton.onClick.AddListener(ShowCustomNight);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        if (startCustomNightButton != null)
            startCustomNightButton.onClick.AddListener(StartCustomNight);
        if (backButton != null)
            backButton.onClick.AddListener(HideCustomNight);

        if (settingsBackButton != null)
            settingsBackButton.onClick.AddListener(HideSettings);
    }

    private void InitializeSettings()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = 1f;
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }

        if (ambientVolumeSlider != null)
        {
            ambientVolumeSlider.value = 0.7f;
            ambientVolumeSlider.onValueChanged.AddListener(OnAmbientVolumeChanged);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = 1f;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }

    private void StartNight(int night)
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();

        if (gameManager != null)
        {
            gameManager.StartNight(night);
        }
    }

    private void ShowCustomNight()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();

        if (customNightPanel != null)
            customNightPanel.SetActive(true);
    }

    private void HideCustomNight()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();

        if (customNightPanel != null)
            customNightPanel.SetActive(false);
    }

    private void StartCustomNight()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();

        // Get AI levels from sliders
        int bonnieAI = bonnieAISlider != null ? Mathf.RoundToInt(bonnieAISlider.value) : 0;
        int chicaAI = chicaAISlider != null ? Mathf.RoundToInt(chicaAISlider.value) : 0;
        int freddyAI = freddyAISlider != null ? Mathf.RoundToInt(freddyAISlider.value) : 0;
        int foxyAI = foxyAISlider != null ? Mathf.RoundToInt(foxyAISlider.value) : 0;

        if (gameManager != null)
        {
            gameManager.StartNight(6); // Custom night = 6
            SetCustomNightAI(bonnieAI, chicaAI, freddyAI, foxyAI);
        }
    }

    private void SetCustomNightAI(int bonnie, int chica, int freddy, int foxy)
    {
        AnimatronicBase[] animatronics = FindObjectsOfType<AnimatronicBase>();
        foreach (var animatronic in animatronics)
        {
            if (animatronic.name.Contains("Bonnie"))
                animatronic.SetAILevel(bonnie);
            else if (animatronic.name.Contains("Chica"))
                animatronic.SetAILevel(chica);
            else if (animatronic.name.Contains("Freddy"))
                animatronic.SetAILevel(freddy);
            else if (animatronic.name.Contains("Foxy"))
                animatronic.SetAILevel(foxy);
        }
    }

    private void ShowSettings()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();

        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    private void HideSettings()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (audioManager != null)
            audioManager.SetMasterVolume(value);
    }

    private void OnAmbientVolumeChanged(float value)
    {
        if (audioManager != null)
            audioManager.SetAmbientVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (audioManager != null)
            audioManager.SetSFXVolume(value);
    }

    private void QuitGame()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();

        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

