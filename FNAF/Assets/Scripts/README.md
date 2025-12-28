# FNAF 1 Unity Game - Setup Guide

## Project Structure

All scripts are organized in the `Assets/Scripts` folder:

- **Core Systems**: GameManager, PowerSystem, CameraSystem, DoorSystem, LightSystem
- **Animatronic AI**: AnimatronicBase and individual animatronic scripts (Bonnie, Chica, Freddy, Foxy)
- **UI Systems**: MainMenuUI, GameUI, CameraUI, PowerUI
- **Support Systems**: AudioManager, JumpscareSystem, VisualEffects

## Setup Instructions

### 1. Scene Setup

1. Create a new scene or use the existing scene
2. Create an empty GameObject named "GameManager" and attach the `GameManager` script
3. Create empty GameObjects for each system:
   - "PowerSystem" with `PowerSystem` script
   - "CameraSystem" with `CameraSystem` script
   - "DoorSystem" with `DoorSystem` script
   - "LightSystem" with `LightSystem` script
   - "AudioManager" with `AudioManager` script
   - "JumpscareSystem" with `JumpscareSystem` script

### 2. Office Setup

1. Create the office environment (walls, desk, etc.)
2. Add two doors (left and right) - assign them in DoorSystem
3. Add two lights (left and right hallways) - assign them in LightSystem
4. Position the main camera at player eye level (approximately Y = 1.6)

### 3. Camera System Setup

1. Create 11 cameras for each camera location:

   - CAM 1A: Show Stage
   - CAM 1B: Dining Area
   - CAM 1C: Pirate Cove
   - CAM 2A: West Hall
   - CAM 2B: West Hall Corner
   - CAM 3: Supply Closet
   - CAM 4A: East Hall
   - CAM 4B: East Hall Corner
   - CAM 5: Backstage
   - CAM 6: Kitchen
   - CAM 7: Restrooms

2. For each camera:
   - Create a Camera GameObject
   - Create a RenderTexture (256x256 or 512x512)
   - Assign the RenderTexture to the camera's Target Texture
   - In CameraSystem, add each camera using `AddCamera()` or configure in inspector

### 4. Animatronic Setup

1. Create GameObjects for each animatronic:

   - "Bonnie" with `BonnieAI` script
   - "Chica" with `ChicaAI` script
   - "Freddy" with `FreddyAI` script
   - "Foxy" with `FoxyAI` script

2. Position them at their starting locations
3. The AI will handle movement automatically

### 5. UI Setup

1. Create a Canvas for the game UI
2. Create UI elements:

   - Power bar (Image with Image Type = Filled)
   - Time text (Text component)
   - Night text (Text component)
   - Camera display (RawImage for camera feed)
   - Game Over panel
   - Night Complete panel
   - Pause menu

3. Assign UI references in:
   - `GameUI` script
   - `CameraUI` script
   - `PowerUI` script
   - `MainMenuUI` script

### 6. Audio Setup

1. Import audio files into `Assets/Audio` folders:

   - Ambient sounds
   - Animatronic movement sounds
   - Jumpscare sounds
   - UI sounds

2. Assign audio clips in the `AudioManager` component

### 7. Jumpscare Setup

1. Create a Canvas for jumpscare overlay
2. Add an Image component for jumpscare sprite
3. Import jumpscare sprites for each animatronic
4. Assign sprites in `JumpscareSystem`

## Controls

- **C** or **Mouse Scroll Up**: Toggle camera tablet
- **1-7**: Switch cameras (when tablet is up)
- **Arrow Keys**: Navigate cameras
- **Q**: Toggle left door
- **E**: Toggle right door
- **A** or **Left Mouse**: Toggle left light
- **D** or **Right Mouse**: Toggle right light
- **ESC**: Pause menu

## Gameplay Balance

The following values can be adjusted in the Inspector:

### PowerSystem

- `baseDrainRate`: Base power drain per second (default: 1%)
- `doorDrainRate`: Power drain per door per second (default: 1%)
- `lightDrainRate`: Power drain per light per second (default: 1%)
- `cameraDrainRate`: Power drain per camera per second (default: 0.5%)

### Animatronic AI

- `aiLevel`: Difficulty level (0-20)
- `moveCooldown`: Time between move attempts
- `minMoveDelay` / `maxMoveDelay`: Random delay range
- `attackTimer`: Time before attack when at door

### GameManager

- `nightDuration`: Length of each night in seconds (default: 360 = 6 minutes)

## Testing

1. Start with Night 1 to test basic mechanics
2. Gradually increase difficulty
3. Adjust power consumption rates if nights are too easy/hard
4. Tune animatronic AI levels for appropriate challenge
5. Test custom night with various AI combinations

## Notes

- All systems are designed to work together
- GameManager coordinates all subsystems
- Power system automatically drains based on active systems
- Animatronics use state machines for behavior
- Camera system detects animatronic positions for static effects
