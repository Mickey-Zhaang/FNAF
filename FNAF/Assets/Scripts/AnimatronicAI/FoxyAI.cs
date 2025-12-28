using UnityEngine;

public class FoxyAI : AnimatronicBase
{
    [Header("Foxy Specific")]
    [SerializeField] private int stage = 0; // 0-4 (0 = hidden, 4 = running)
    [SerializeField] private float stageAdvanceTime = 5f;
    [SerializeField] private float runCooldown = 20f;
    [SerializeField] private bool hasRun = false;

    private float stageTimer = 0f;
    private float cooldownTimer = 0f;

    protected override void Start()
    {
        base.Start();
        animatronicName = "Foxy";
        currentLocation = AnimatronicLocation.PirateCove;
    }

    protected override void Update()
    {
        base.Update();

        if (gameManager != null && gameManager.currentState != GameState.Playing)
            return;

        UpdateFoxyStage();
    }

    protected override void UpdateIdle()
    {
        // Foxy has special mechanics - he advances stages in Pirate Cove
        if (currentLocation == AnimatronicLocation.PirateCove)
        {
            // Stage advancement is handled in UpdateFoxyStage
            return;
        }

        base.UpdateIdle();
    }

    private void UpdateFoxyStage()
    {
        if (currentLocation != AnimatronicLocation.PirateCove)
            return;

        // Check if Foxy is being watched
        bool isBeingWatched = IsBeingWatched();

        if (isBeingWatched)
        {
            // Reset stage if being watched
            stage = 0;
            stageTimer = 0f;
            return;
        }

        // Advance stage if not being watched
        if (stage < 4)
        {
            stageTimer += Time.deltaTime;

            if (stageTimer >= stageAdvanceTime)
            {
                stage++;
                stageTimer = 0f;

                if (audioManager != null)
                {
                    audioManager.PlayFoxySound(stage);
                }
            }
        }
        else if (stage == 4 && !hasRun)
        {
            // Foxy runs to the office
            RunToOffice();
        }

        // Cooldown after running
        if (hasRun)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= runCooldown)
            {
                hasRun = false;
                stage = 0;
                cooldownTimer = 0f;
            }
        }
    }

    private bool IsBeingWatched()
    {
        if (cameraSystem == null)
            return false;

        if (cameraSystem.IsCameraActive() &&
            cameraSystem.GetCurrentCamera() == CameraLocation.CAM_1C)
        {
            return true;
        }

        return false;
    }

    private void RunToOffice()
    {
        hasRun = true;
        cooldownTimer = 0f;
        currentLocation = AnimatronicLocation.Office;
        currentState = AnimatronicState.Attacking;

        if (audioManager != null)
        {
            audioManager.PlayFoxyRunSound();
        }

        // Foxy attacks immediately
        Attack();
    }

    protected override void MoveToNextLocation()
    {
        // Foxy doesn't use normal movement - he stays in Pirate Cove
        // and runs when stage 4 is reached
    }

    protected override DoorSide GetDoorSide()
    {
        return DoorSide.Left; // Foxy comes from left
    }

    protected override LightSide GetLightSide()
    {
        return LightSide.Left;
    }

    protected override CameraLocation ConvertLocationToCamera(AnimatronicLocation location)
    {
        if (location == AnimatronicLocation.PirateCove)
        {
            return CameraLocation.CAM_1C;
        }
        return CameraLocation.None;
    }

    protected override bool ShouldPauseWhenViewed()
    {
        // Foxy resets when viewed, doesn't pause
        return false;
    }

    public int GetStage()
    {
        return stage;
    }

    public bool HasRun()
    {
        return hasRun;
    }

    public override void ResetPosition()
    {
        base.ResetPosition();
        currentLocation = AnimatronicLocation.PirateCove;
        stage = 0;
        stageTimer = 0f;
        hasRun = false;
        cooldownTimer = 0f;
    }
}

