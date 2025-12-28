using UnityEngine;
using System.Collections;

public class JumpscareSystem : MonoBehaviour
{
    [Header("Jumpscare Settings")]
    [SerializeField] private float jumpscareDuration = 2f;
    [SerializeField] private float screenShakeIntensity = 0.5f;
    [SerializeField] private float screenShakeDuration = 1f;

    [Header("Jumpscare Visuals")]
    [SerializeField] private GameObject jumpscareCanvas;
    [SerializeField] private UnityEngine.UI.Image jumpscareImage;
    [SerializeField] private Sprite bonnieJumpscareSprite;
    [SerializeField] private Sprite chicaJumpscareSprite;
    [SerializeField] private Sprite freddyJumpscareSprite;
    [SerializeField] private Sprite foxyJumpscareSprite;

    [Header("Camera Shake")]
    [SerializeField] private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private bool isShaking = false;

    private GameManager gameManager;
    private AudioManager audioManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioManager = FindObjectOfType<AudioManager>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera != null)
            originalCameraPosition = mainCamera.transform.localPosition;

        if (jumpscareCanvas != null)
            jumpscareCanvas.SetActive(false);
    }

    public void TriggerJumpscare(string animatronicName)
    {
        StartCoroutine(PlayJumpscare(animatronicName));
    }

    private IEnumerator PlayJumpscare(string animatronicName)
    {
        // Get the appropriate sprite
        Sprite jumpscareSprite = GetJumpscareSprite(animatronicName);

        // Show jumpscare image
        if (jumpscareCanvas != null)
        {
            jumpscareCanvas.SetActive(true);
            if (jumpscareImage != null && jumpscareSprite != null)
            {
                jumpscareImage.sprite = jumpscareSprite;
                jumpscareImage.color = Color.white;
            }
        }

        // Play jumpscare sound
        if (audioManager != null)
        {
            audioManager.PlayJumpscareSound(animatronicName);
        }

        // Screen shake
        StartCoroutine(ScreenShake());

        // Wait for jumpscare duration
        yield return new WaitForSeconds(jumpscareDuration);

        // Hide jumpscare
        if (jumpscareCanvas != null)
        {
            jumpscareCanvas.SetActive(false);
        }

        // Stop screen shake
        StopAllCoroutines();
        if (mainCamera != null)
        {
            mainCamera.transform.localPosition = originalCameraPosition;
        }

        // Trigger game over
        if (gameManager != null)
        {
            gameManager.GameOver("Jumpscare");
        }
    }

    private Sprite GetJumpscareSprite(string animatronicName)
    {
        switch (animatronicName)
        {
            case "Bonnie":
                return bonnieJumpscareSprite;
            case "Chica":
                return chicaJumpscareSprite;
            case "Freddy":
                return freddyJumpscareSprite;
            case "Foxy":
                return foxyJumpscareSprite;
            default:
                return bonnieJumpscareSprite;
        }
    }

    private IEnumerator ScreenShake()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < screenShakeDuration && mainCamera != null)
        {
            float x = Random.Range(-1f, 1f) * screenShakeIntensity;
            float y = Random.Range(-1f, 1f) * screenShakeIntensity;
            mainCamera.transform.localPosition = originalCameraPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (mainCamera != null)
        {
            mainCamera.transform.localPosition = originalCameraPosition;
        }
        isShaking = false;
    }

    public bool IsJumpscareActive()
    {
        return jumpscareCanvas != null && jumpscareCanvas.activeSelf;
    }
}

