using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CameraUI : MonoBehaviour
{
    [Header("Camera Display")]
    [SerializeField] private RawImage cameraDisplay;
    [SerializeField] private GameObject staticOverlay;
    [SerializeField] private Image staticImage;

    [Header("Camera Buttons")]
    [SerializeField] private List<Button> cameraButtons = new List<Button>();
    [SerializeField] private Button tabletToggleButton;

    [Header("Camera Labels")]
    [SerializeField] private Text currentCameraText;
    [SerializeField] private Text cameraNameText;

    private CameraSystem cameraSystem;
    private AudioManager audioManager;

    private void Start()
    {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
        audioManager = FindFirstObjectByType<AudioManager>();

        SetupButtons();
    }

    private void SetupButtons()
    {
        if (tabletToggleButton != null)
            tabletToggleButton.onClick.AddListener(ToggleTablet);

        // Setup camera buttons (if using button-based navigation)
        for (int i = 0; i < cameraButtons.Count && i < 11; i++)
        {
            int cameraIndex = i;
            cameraButtons[i].onClick.AddListener(() => SwitchToCamera(cameraIndex));
        }
    }

    private void Update()
    {
        UpdateCameraDisplay();
        UpdateCameraInfo();
    }

    private void UpdateCameraDisplay()
    {
        if (cameraSystem == null || cameraDisplay == null)
            return;

        bool isTabletUp = cameraSystem.IsTabletUp();

        // Show/hide camera display based on tablet state
        cameraDisplay.gameObject.SetActive(isTabletUp);
    }

    private void UpdateCameraInfo()
    {
        if (cameraSystem == null)
            return;

        if (currentCameraText != null)
        {
            CameraLocation cam = cameraSystem.GetCurrentCamera();
            if (cam != CameraLocation.None)
            {
                currentCameraText.text = "CAM " + GetCameraLabel(cam);
            }
            else
            {
                currentCameraText.text = "";
            }
        }

        if (cameraNameText != null)
        {
            CameraLocation cam = cameraSystem.GetCurrentCamera();
        }
    }

    private string GetCameraLabel(CameraLocation location)
    {
        switch (location)
        {
            case CameraLocation.CAM_1A: return "1A";
            case CameraLocation.CAM_1B: return "1B";
            case CameraLocation.CAM_2A: return "2A";
            case CameraLocation.CAM_2B: return "2B";
            case CameraLocation.CAM_3A: return "3A";
            case CameraLocation.CAM_3B: return "3B";
            case CameraLocation.CAM_3C: return "3C";
            case CameraLocation.CAM_4A: return "4A";
            case CameraLocation.CAM_4B: return "4B";
            case CameraLocation.CAM_4C: return "4C";
            case CameraLocation.CAM_5A: return "5A";
            case CameraLocation.CAM_5B: return "5B";
            case CameraLocation.CAM_5C: return "5C";
            default: return "";
        }
    }

    private void ToggleTablet()
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();

        if (cameraSystem != null)
        {
            cameraSystem.ToggleTablet();
            if (audioManager != null && cameraSystem.IsTabletUp())
            {
                audioManager.PlayCameraStatic();
            }
        }
    }

    private void SwitchToCamera(int index)
    {
        if (audioManager != null)
            audioManager.PlayButtonClick();

        CameraLocation[] locations = {
            CameraLocation.CAM_1A, CameraLocation.CAM_1B,
            CameraLocation.CAM_2A, CameraLocation.CAM_2B,
            CameraLocation.CAM_3A, CameraLocation.CAM_3B, CameraLocation.CAM_3C,
            CameraLocation.CAM_4A, CameraLocation.CAM_4B, CameraLocation.CAM_4C,
            CameraLocation.CAM_5A, CameraLocation.CAM_5B, CameraLocation.CAM_5C,
        };

        if (index >= 0 && index < locations.Length && cameraSystem != null)
        {
            cameraSystem.SwitchCamera(locations[index]);
        }
    }
}

