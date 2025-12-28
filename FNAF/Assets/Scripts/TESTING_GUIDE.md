# Testing Guide - Quick Start

This guide will help you set up and test the FNAF game in Unity.

## Step 1: Open Your Scene

1. Open Unity Editor
2. Open the scene: `Assets/Scenes/SampleScene.unity` (or create a new scene)

## Step 2: Create the Core GameObjects

### Create GameManager

1. Right-click in Hierarchy → Create Empty
2. Name it "GameManager"
3. Add Component → Search for "GameManager" → Add it

### Create System Objects

Repeat for each system:

1. Right-click in Hierarchy → Create Empty
2. Name it (e.g., "PowerSystem")
3. Add the corresponding script component:
   - PowerSystem
   - CameraSystem
   - DoorSystem
   - LightSystem
   - AudioManager
   - JumpscareSystem

## Step 3: Set Up the Office (Minimal Test Setup)

### Create a Simple Office

1. Create a Plane (GameObject → 3D Object → Plane) - this is your floor
2. Scale it to (10, 1, 10)
3. Create a Cube for the desk (GameObject → 3D Object → Cube)
4. Position the Main Camera at (0, 1.6, 0) - this is player eye level

### Set Up Doors (Simple Test)

1. Create two Cubes for doors:
   - Left Door: Position (-2, 0, 0), Scale (0.1, 2, 1)
   - Right Door: Position (2, 0, 0), Scale (0.1, 2, 1)
2. In DoorSystem component:
   - Drag Left Door to "Left Door" field
   - Drag Right Door to "Right Door" field

### Set Up Lights (Simple Test)

1. Create two Lights (GameObject → Light → Directional Light)
   - Left Light: Position (-2, 2, 0)
   - Right Light: Position (2, 2, 0)
2. In LightSystem component:
   - Drag Left Light to "Left Light" field
   - Drag Right Light to "Right Light" field

## Step 4: Connect Systems in GameManager

1. Select GameManager in Hierarchy
2. In Inspector, drag system objects to their fields:
   - Power System → PowerSystem field
   - Camera System → CameraSystem field
   - Door System → DoorSystem field
   - Light System → LightSystem field
   - Audio Manager → AudioManager field
   - Jumpscare System → JumpscareSystem field

## Step 5: Create Basic UI (Minimal Test)

### Create Canvas

1. Right-click in Hierarchy → UI → Canvas
2. This creates a Canvas automatically

### Create Power Display (Quick Test)

1. Right-click Canvas → UI → Text
2. Name it "PowerText"
3. Position it at top-left
4. In GameUI script (you'll need to add this to a GameObject):
   - Drag PowerText to "Power Text" field

### Alternative: Test Without Full UI

You can test the core systems without UI first:

- Press Play
- Check Console for any errors
- Systems should initialize

## Step 6: Test Basic Functionality

### Test Power System

1. Press Play
2. Open Console (Window → General → Console)
3. Power should start at 100%
4. Check Console for any errors

### Test Door System

1. While playing, press **Q** (left door) and **E** (right door)
2. Doors should toggle (you'll see them in Scene view)
3. Check Console for door state changes

### Test Light System

1. Press **A** (left light) and **D** (right light)
2. Lights should toggle on/off
3. Check Console for light state changes

### Test Camera System

1. Press **C** to toggle camera tablet
2. You'll need cameras set up first (see below)

## Step 7: Quick Camera Test (Optional)

For a basic camera test:

1. Create a Camera (GameObject → Camera)
2. Position it somewhere in the scene
3. Create a RenderTexture (Assets → Create → Render Texture)
4. Assign RenderTexture to camera's Target Texture
5. In CameraSystem, you can add cameras programmatically or set up in Inspector

## Step 8: Test Animatronics (Minimal)

### Create Test Animatronic

1. Create Empty GameObject → Name it "Bonnie"
2. Add Component → BonnieAI
3. Press Play
4. Check Console - animatronic should initialize

### Test Animatronic Movement

1. In BonnieAI Inspector, set AI Level to 10 (for testing)
2. Press Play
3. Watch Console for movement logs (you may need to add Debug.Log statements)

## Step 9: Test GameManager

### Test Night Start

1. Select GameManager
2. In Inspector, find "Current Night" or call `StartNight(1)` from code
3. Or create a simple test button:
   ```csharp
   // Add this to a test script
   void Update() {
       if (Input.GetKeyDown(KeyCode.Space)) {
           GameManager.Instance.StartNight(1);
       }
   }
   ```

## Step 10: Debug and Verify

### Check Console

- Look for any errors or warnings
- All systems should initialize without errors

### Test Input

- Q/E for doors
- A/D for lights
- C for camera tablet
- ESC for pause

### Verify Systems Are Connected

1. Select GameManager
2. Check that all system references are assigned (not "None")
3. If any are missing, drag the system objects to the fields

## Quick Test Script

Create a simple test script to verify everything works:

```csharp
using UnityEngine;

public class QuickTest : MonoBehaviour
{
    void Update()
    {
        // Start night with Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.StartNight(1);
                Debug.Log("Night 1 Started!");
            }
        }

        // Print power level with P
        if (Input.GetKeyDown(KeyCode.P))
        {
            PowerSystem ps = FindObjectOfType<PowerSystem>();
            if (ps != null)
            {
                Debug.Log("Power: " + ps.GetPowerPercentage() + "%");
            }
        }
    }
}
```

1. Create Empty GameObject
2. Add this script
3. Press Play
4. Press Space to start night
5. Press P to check power

## Common Issues and Solutions

### "NullReferenceException"

- Make sure all system references are assigned in GameManager
- Check that system objects exist in the scene

### "System not found"

- Make sure system scripts are attached to GameObjects
- Check that GameObjects are active (not disabled)

### "Input not working"

- Make sure you're in Play mode
- Check that no other window has focus
- Verify input keys in the scripts

### "Power not draining"

- Check that PowerSystem is running
- Verify door/light systems are connected
- Check Console for errors

## Next Steps After Basic Testing

Once basic systems work:

1. Set up proper cameras with RenderTextures
2. Create proper UI with Canvas
3. Add animatronic models/visuals
4. Import and assign audio files
5. Create jumpscare sprites
6. Build out the full office environment

## Testing Checklist

- [ ] All systems initialize without errors
- [ ] Power system drains over time
- [ ] Doors toggle with Q/E
- [ ] Lights toggle with A/D
- [ ] Camera tablet toggles with C
- [ ] GameManager can start a night
- [ ] Animatronics initialize
- [ ] No NullReferenceExceptions in Console

Start with the basic setup, test each system individually, then connect them together!
