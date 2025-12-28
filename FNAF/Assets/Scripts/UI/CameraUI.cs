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
        cameraSystem = FindObjectOfType<CameraSystem>();
        audioManager = FindObjectOfType<AudioManager>();

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
        UpdateStaticEffect();
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

    private void UpdateStaticEffect()
    {
        if (cameraSystem == null || staticOverlay == null)
            return;

        CameraLocation currentCam = cameraSystem.GetCurrentCamera();
        bool hasStatic = cameraSystem.IsAnimatronicVisible(currentCam, ""); // Check if any animatronic is visible

        staticOverlay.SetActive(hasStatic && cameraSystem.IsTabletUp());

        // Animate static effect
        if (staticImage != null && hasStatic)
        {
            // Randomize static opacity for flicker effect
            Color c = staticImage.color;
            c.a = Random.Range(0.3f, 0.7f);
            staticImage.color = c;
        }
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
            cameraNameText.text = GetCameraName(cam);
        }
    }

    private string GetCameraLabel(CameraLocation location)
    {
        switch (location)
        {
            case CameraLocation.CAM_1A: return "1A";
            case CameraLocation.CAM_1B: return "1B";
            case CameraLocation.CAM_1C: return "1C";
            case CameraLocation.CAM_2A: return "2A";
            case CameraLocation.CAM_2B: return "2B";
            case CameraLocation.CAM_3: return "3";
            case CameraLocation.CAM_4A: return "4A";
            case CameraLocation.CAM_4B: return "4B";
            case CameraLocation.CAM_5: return "5";
            case CameraLocation.CAM_6: return "6";
            case CameraLocation.CAM_7: return "7";
            default: return "";
        }
    }

    private string GetCameraName(CameraLocation location)
    {
        switch (location)
        {
            case CameraLocation.CAM_1A: return "SHOW STAGE";
            case CameraLocation.CAM_1B: return "DINING AREA";
            case CameraLocation.CAM_1C: return "PIRATE COVE";
            case CameraLocation.CAM_2A: return "WEST HALL";
            case CameraLocation.CAM_2B: return "WEST HALL CORNER";
            case CameraLocation.CAM_3: return "SUPPLY CLOSET";
            case CameraLocation.CAM_4A: return "EAST HALL";
            case CameraLocation.CAM_4B: return "EAST HALL CORNER";
            case CameraLocation.CAM_5: return "BACKSTAGE";
            case CameraLocation.CAM_6: return "KITCHEN";
            case CameraLocation.CAM_7: return "RESTROOMS";
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
            CameraLocation.CAM_1A, CameraLocation.CAM_1B, CameraLocation.CAM_1C,
            CameraLocation.CAM_2A, CameraLocation.CAM_2B, CameraLocation.CAM_3,
            CameraLocation.CAM_4A, CameraLocation.CAM_4B, CameraLocation.CAM_5,
            CameraLocation.CAM_6, CameraLocation.CAM_7
        };

        if (index >= 0 && index < locations.Length && cameraSystem != null)
        {
            cameraSystem.SwitchCamera(locations[index]);
        }
    }
}

