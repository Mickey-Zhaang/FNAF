using UnityEngine;

/// <summary>
/// Bare bones Foxy template - self-contained with no external dependencies.
/// </summary>
public class FoxyAI : AnimatronicBase
{
    [Header("Foxy Specific")]
    [SerializeField] private int stage = 0; // 0-4 (0 = hidden, 4 = running)

    protected override void Start()
    {
        base.Start();
        animatronicName = "Foxy";
        currentLocation = AnimatronicLocation.PirateCove;
    }

    protected override void MoveToNextLocation()
    {
        // TODO: Implement Foxy's movement logic here
        // Foxy typically stays in Pirate Cove and runs when stage 4 is reached
        Debug.Log($"{animatronicName} stage: {stage}");
    }

    protected override void UpdateAttacking()
    {
        base.UpdateAttacking();
        Debug.Log($"{animatronicName} runs to office!");
    }

    public override void ResetPosition()
    {
        base.ResetPosition();
        currentLocation = AnimatronicLocation.PirateCove;
        stage = 0;
    }

    public int GetStage()
    {
        return stage;
    }
}
