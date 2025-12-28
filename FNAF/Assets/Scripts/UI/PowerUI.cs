using UnityEngine;
using UnityEngine.UI;

public class PowerUI : MonoBehaviour
{
    [Header("Power Display")]
    [SerializeField] private Text powerText;
    [SerializeField] private Image powerBar;
    [SerializeField] private Image powerBarBackground;

    [Header("Power Warning")]
    [SerializeField] private GameObject lowPowerWarning;
    [SerializeField] private float lowPowerThreshold = 25f;
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowColor = Color.yellow;
    [SerializeField] private Color criticalColor = Color.red;

    private PowerSystem powerSystem;

    private void Start()
    {
        powerSystem = FindObjectOfType<PowerSystem>();

        if (powerSystem != null)
        {
            powerSystem.OnPowerChanged.AddListener(UpdatePowerDisplay);
            powerSystem.OnPowerDepleted.AddListener(OnPowerDepleted);
        }

        if (lowPowerWarning != null)
            lowPowerWarning.SetActive(false);
    }

    private void Update()
    {
        UpdatePowerDisplay(powerSystem != null ? powerSystem.GetPowerLevel() : 100f);
    }

    private void UpdatePowerDisplay(float powerLevel)
    {
        float powerPercentage = powerSystem != null ? powerSystem.GetPowerPercentage() : 100f;

        // Update power text
        if (powerText != null)
        {
            powerText.text = Mathf.RoundToInt(powerPercentage) + "%";
        }

        // Update power bar
        if (powerBar != null)
        {
            powerBar.fillAmount = powerPercentage / 100f;

            // Change color based on power level
            if (powerPercentage > lowPowerThreshold)
            {
                powerBar.color = normalColor;
            }
            else if (powerPercentage > lowPowerThreshold / 2f)
            {
                powerBar.color = lowColor;
            }
            else
            {
                powerBar.color = criticalColor;
            }
        }

        // Show/hide low power warning
        if (lowPowerWarning != null)
        {
            lowPowerWarning.SetActive(powerPercentage <= lowPowerThreshold);
        }
    }

    private void OnPowerDepleted()
    {
        if (lowPowerWarning != null)
            lowPowerWarning.SetActive(true);

        // Power depleted effect
        if (powerBar != null)
            powerBar.color = criticalColor;
    }

    private void OnDestroy()
    {
        if (powerSystem != null)
        {
            powerSystem.OnPowerChanged.RemoveListener(UpdatePowerDisplay);
            powerSystem.OnPowerDepleted.RemoveListener(OnPowerDepleted);
        }
    }
}

