# FNAF Visual Setup Guide

This guide will help you create proper game scenes and visuals for your Five Nights at Freddy's game, replacing the default blue screen with a fully functional office environment.

## Table of Contents

1. [Office Scene Setup](#office-scene-setup)
2. [Camera System Visuals](#camera-system-visuals)
3. [Door System Visuals](#door-system-visuals)
4. [Light System Visuals](#light-system-visuals)
5. [Animatronic Visuals](#animatronic-visuals)
6. [UI Setup](#ui-setup)
7. [Materials and Textures](#materials-and-textures)
8. [Lighting Setup](#lighting-setup)
9. [Post-Processing Effects](#post-processing-effects)

---

## Office Scene Setup

### Basic Office Structure

1. **Create the Office Floor**

   - GameObject → 3D Object → Plane
   - Name: "OfficeFloor"
   - Scale: (10, 1, 10)
   - Position: (0, 0, 0)
   - Add a material (dark wood or carpet texture)

2. **Create Office Walls**

   - GameObject → 3D Object → Cube
   - Create 4 walls:
     - Back Wall: Position (0, 2, -5), Scale (10, 4, 0.2)
     - Left Wall: Position (-5, 2, 0), Scale (0.2, 4, 10)
     - Right Wall: Position (5, 2, 0), Scale (0.2, 4, 10)
     - Front Wall (with doorways): Position (0, 2, 5), Scale (10, 4, 0.2)
   - Apply a wall material (beige or gray)

3. **Create Desk**

   - GameObject → 3D Object → Cube
   - Name: "Desk"
   - Position: (0, 0.5, 0)
   - Scale: (3, 1, 1.5)
   - This is where the player sits

4. **Create Hallways**
   - Left Hallway: Cube at Position (-2, 1, 2), Scale (1, 2, 3)
   - Right Hallway: Cube at Position (2, 1, 2), Scale (1, 2, 3)
   - These are the areas visible when lights are on

### Player Camera Setup

1. **Main Camera (Player View)**

   - Select Main Camera
   - Position: (0, 1.6, 0) - Eye level
   - Rotation: (0, 0, 0) - Looking forward
   - Field of View: 60-75 degrees
   - This represents the player's view from the desk

2. **Camera Restrictions**
   - The player cannot move - this is a fixed camera position
   - All interaction happens from this viewpoint

---

## Camera System Visuals

### Setting Up Security Cameras

Each camera needs to be set up with a RenderTexture to display on the tablet.

#### Step 1: Create RenderTextures

1. In Project window: Right-click → Create → Render Texture
2. Create 11 RenderTextures (one for each camera):

   - `CAM_1A_RenderTexture` (Show Stage)
   - `CAM_1B_RenderTexture` (Dining Area)
   - `CAM_1C_RenderTexture` (Pirate Cove)
   - `CAM_2A_RenderTexture` (West Hall)
   - `CAM_2B_RenderTexture` (West Hall Corner)
   - `CAM_3_RenderTexture` (Supply Closet)
   - `CAM_4A_RenderTexture` (East Hall)
   - `CAM_4B_RenderTexture` (East Hall Corner)
   - `CAM_5_RenderTexture` (Backstage)
   - `CAM_6_RenderTexture` (Kitchen)
   - `CAM_7_RenderTexture` (Restrooms)

3. For each RenderTexture:
   - Size: 512x512 or 256x256 (for performance)
   - Depth Buffer: 16 bit depth
   - Color Format: ARGB32

#### Step 2: Create Camera GameObjects

For each camera location:

1. **Create Camera GameObject**

   - GameObject → Camera
   - Name: "CAM_1A" (or appropriate name)
   - Position: Set based on location (see locations below)
   - Rotation: Point camera at the area

2. **Assign RenderTexture**

   - In Camera component, set "Target Texture" to the corresponding RenderTexture
   - Disable "Audio Listener" (only Main Camera should have this)

3. **Camera Settings**
   - Field of View: 60-75
   - Culling Mask: Set to appropriate layers
   - Clear Flags: Solid Color (dark gray/black)

#### Step 3: Camera Locations

Position cameras at these locations:

- **CAM 1A (Show Stage)**: Position (0, 2, -8), Look at stage area
- **CAM 1B (Dining Area)**: Position (0, 3, -10), Look down at dining area
- **CAM 1C (Pirate Cove)**: Position (-3, 2, -8), Look at Foxy's area
- **CAM 2A (West Hall)**: Position (-2, 2, 2), Look down hallway
- **CAM 2B (West Hall Corner)**: Position (-2, 2, 4), Look at corner
- **CAM 3 (Supply Closet)**: Position (-4, 2, 0), Look at closet
- **CAM 4A (East Hall)**: Position (2, 2, 2), Look down hallway
- **CAM 4B (East Hall Corner)**: Position (2, 2, 4), Look at corner
- **CAM 5 (Backstage)**: Position (4, 2, -6), Look at backstage
- **CAM 6 (Kitchen)**: Position (0, 2, -12), Look at kitchen (no visual, just audio)
- **CAM 7 (Restrooms)**: Position (3, 2, -8), Look at restrooms

#### Step 4: Connect to CameraSystem

1. Select the GameObject with `CameraSystem` script
2. In Inspector, expand "Cameras" list
3. For each camera:
   - Click "+" to add new CameraData entry
   - Set Location dropdown (CAM_1A, CAM_1B, etc.)
   - Set Display Name
   - Drag Camera GameObject to "Camera" field
   - Drag RenderTexture to "Render Texture" field

#### Step 5: Create Camera Tablet UI

1. **Create Tablet Canvas**

   - Right-click Hierarchy → UI → Canvas
   - Name: "CameraTablet"
   - Canvas Scaler: Scale With Screen Size
   - Reference Resolution: 1920x1080

2. **Create Camera Display**

   - Right-click Canvas → UI → Raw Image
   - Name: "CameraDisplay"
   - Anchor: Center, stretch to fill most of screen
   - Position: (0, -100, 0) - Lower portion of screen
   - Size: Match your tablet design

3. **Create Static Overlay**

   - Right-click Canvas → UI → Image
   - Name: "StaticOverlay"
   - Set as child of CameraDisplay
   - Anchor: Stretch to fill parent
   - Material: Create a static/noise material (see Materials section)
   - Color: White with low alpha (0.3-0.5)
   - Set to inactive by default

4. **Connect to CameraSystem**
   - In CameraSystem component:
     - Drag CameraDisplay to "Camera Display" field
     - Drag StaticOverlay to "Static Overlay" field (if you have a static image component)
     - Drag CameraTablet GameObject to "Tablet UI" field

---

## Door System Visuals

### Creating Door Models

#### Left Door

1. **Create Door GameObject**

   - GameObject → 3D Object → Cube
   - Name: "LeftDoor"
   - Position: (-2, 1, 1.5) - In left doorway
   - Scale: (0.1, 2, 1.5)
   - Add a dark material (wood or metal)

2. **Create Door Button**

   - GameObject → 3D Object → Cube (or use a UI Button)
   - Name: "LeftDoorButton"
   - Position: Near left side of desk
   - Scale: (0.2, 0.2, 0.2)
   - Add a red material or use UI Button

3. **Door Animation (Optional)**
   - Add Animator component to LeftDoor
   - Create animation: Door slides down/up
   - Or use simple position changes in code

#### Right Door

1. **Create Door GameObject**

   - GameObject → 3D Object → Cube
   - Name: "RightDoor"
   - Position: (2, 1, 1.5) - In right doorway
   - Scale: (0.1, 2, 1.5)
   - Add a dark material

2. **Create Door Button**
   - GameObject → 3D Object → Cube (or UI Button)
   - Name: "RightDoorButton"
   - Position: Near right side of desk
   - Scale: (0.2, 0.2, 0.2)

#### Connect to DoorSystem

1. Select GameObject with `DoorSystem` script
2. In Inspector:
   - Drag LeftDoor to "Left Door" field
   - Drag RightDoor to "Right Door" field
   - Drag LeftDoorButton to "Left Door Button" field
   - Drag RightDoorButton to "Right Door Button" field

**Note**: Doors should be disabled (inactive) when open, enabled (active) when closed. The DoorSystem script handles this automatically.

---

## Light System Visuals

### Creating Hallway Lights

#### Left Light

1. **Create Light GameObject**

   - GameObject → Light → Spot Light (or Point Light)
   - Name: "LeftHallwayLight"
   - Position: (-2, 2.5, 2) - Above left hallway
   - Rotation: (90, 0, 0) - Pointing down
   - Color: Yellow/White
   - Intensity: 2-3
   - Range: 5-10
   - Spot Angle: 45-60 degrees

2. **Create Light Button**
   - GameObject → 3D Object → Cube (or UI Button)
   - Name: "LeftLightButton"
   - Position: Near left side of desk
   - Add emissive material when on

#### Right Light

1. **Create Light GameObject**

   - GameObject → Light → Spot Light
   - Name: "RightHallwayLight"
   - Position: (2, 2.5, 2) - Above right hallway
   - Rotation: (90, 0, 0)
   - Same settings as left light

2. **Create Light Button**
   - GameObject → 3D Object → Cube (or UI Button)
   - Name: "RightLightButton"

#### Connect to LightSystem

1. Select GameObject with `LightSystem` script
2. In Inspector:
   - Drag LeftHallwayLight to "Left Light" field
   - Drag RightHallwayLight to "Right Light" field
   - Drag LeftLightButton to "Left Light Button" field
   - Drag RightLightButton to "Right Light Button" field

**Note**: Lights should be disabled when off. The LightSystem script handles this.

---

## Animatronic Visuals

### Creating Animatronic Models

You can use simple placeholders or import models. Here's how to set up basic animatronic visuals:

#### Bonnie (Left Side)

1. **Create Bonnie GameObject**

   - GameObject → 3D Object → Capsule (or import model)
   - Name: "Bonnie"
   - Position: Starting position (Show Stage area)
   - Scale: Appropriate size
   - Add `BonnieAI` script component

2. **Create Materials**

   - Purple material for Bonnie
   - Add to model

3. **Position Tracking**
   - The AI script handles movement
   - Position will update based on current location
   - You may want to add simple animations or position lerping

#### Chica (Right Side)

1. **Create Chica GameObject**
   - GameObject → 3D Object → Capsule
   - Name: "Chica"
   - Position: Starting position
   - Add `ChicaAI` script component
   - Yellow material

#### Freddy (Center/Right)

1. **Create Freddy GameObject**
   - GameObject → 3D Object → Capsule
   - Name: "Freddy"
   - Position: Starting position
   - Add `FreddyAI` script component
   - Brown material

#### Foxy (Pirate Cove)

1. **Create Foxy GameObject**
   - GameObject → 3D Object → Capsule
   - Name: "Foxy"
   - Position: Pirate Cove area (-3, 0, -8)
   - Add `FoxyAI` script component
   - Red/orange material

### Animatronic Visibility

- Animatronics should only be visible when:
  - On cameras (when viewing that camera)
  - In hallways (when light is on)
  - At doors (when door is open and they're attacking)

You may want to add a visibility script that shows/hides animatronics based on their location and camera/light states.

---

## UI Setup

### Main Game UI

1. **Create Game Canvas**

   - Right-click Hierarchy → UI → Canvas
   - Name: "GameUI"
   - Canvas Scaler: Scale With Screen Size
   - Reference Resolution: 1920x1080

2. **Power Display**

   - Right-click Canvas → UI → Image
   - Name: "PowerBar"
   - Anchor: Top-Left
   - Position: (50, -50, 0)
   - Image Type: Filled
   - Fill Method: Horizontal
   - Color: Green (changes to yellow/red when low)

   - Create background: Duplicate PowerBar, name "PowerBarBackground"
   - Set as sibling, move behind PowerBar
   - Color: Dark gray/black

3. **Time Display**

   - Right-click Canvas → UI → Text - TextMeshPro (or Legacy Text)
   - Name: "TimeText"
   - Anchor: Top-Center
   - Position: (0, -30, 0)
   - Font Size: 24-32
   - Color: White
   - Text: "12:00 AM"

4. **Night Display**

   - Right-click Canvas → UI → Text
   - Name: "NightText"
   - Anchor: Top-Left
   - Position: (50, -20, 0)
   - Font Size: 20-24
   - Text: "NIGHT 1"

5. **Connect to GameUI Script**
   - Create empty GameObject → Name: "GameUI"
   - Add `GameUI` script component
   - Drag UI elements to appropriate fields in Inspector

### Camera Tablet UI

See [Camera System Visuals](#camera-system-visuals) section above for tablet setup.

### Game Over / Night Complete UI

1. **Game Over Panel**

   - Right-click Canvas → UI → Panel
   - Name: "GameOverPanel"
   - Set to inactive by default
   - Add dark background (semi-transparent black)
   - Add Text child: "GAME OVER"
   - Add buttons: Restart, Menu

2. **Night Complete Panel**
   - Similar setup to Game Over
   - Text: "6 AM" or "NIGHT COMPLETE"
   - Buttons: Next Night, Menu

---

## Materials and Textures

### Creating Basic Materials

1. **Office Floor Material**

   - Right-click in Project → Create → Material
   - Name: "OfficeFloorMat"
   - Albedo: Dark brown/gray (or use texture)
   - Smoothness: 0.3-0.5

2. **Wall Material**

   - Create Material: "WallMat"
   - Albedo: Beige/light gray
   - Smoothness: 0.2

3. **Door Material**

   - Create Material: "DoorMat"
   - Albedo: Dark brown/black
   - Metallic: 0.3-0.5

4. **Static/Noise Material (for camera static)**
   - Create Material: "StaticMat"
   - Shader: Unlit/Texture (or use a noise shader)
   - For simple static, use a noise texture or shader graph

### Texture Setup

1. Import textures into `Assets/Textures` folder
2. Set texture import settings:
   - Texture Type: Default
   - Max Size: 1024 or 2048 (depending on quality needs)
   - Compression: Automatic

---

## Lighting Setup

### Scene Lighting

1. **Main Directional Light**

   - GameObject → Light → Directional Light
   - Name: "MainLight"
   - Rotation: (50, -30, 0) - Angled down
   - Intensity: 0.5-1.0 (dim, like a security office)
   - Color: Slightly warm white

2. **Ambient Lighting**

   - Window → Rendering → Lighting
   - Environment → Ambient Source: Skybox or Color
   - Ambient Intensity: 0.2-0.3 (very dim)
   - Ambient Color: Dark blue/gray

3. **Office Lighting**
   - Add a dim point light above the desk
   - Intensity: 0.3-0.5
   - Range: 3-5
   - Color: Warm white/yellow

### Light Settings for Atmosphere

- Keep overall scene dim and atmospheric
- Use warm, dim lighting to create tension
- Hallway lights should be brighter when on (contrast)
- Camera views should be dimly lit

---

## Post-Processing Effects

### Adding Post-Processing

1. **Install Post-Processing Package** (if not already installed)

   - Window → Package Manager
   - Search "Post Processing"
   - Install Universal RP Post Processing (if using URP)

2. **Create Post-Processing Volume**

   - GameObject → Volume → Global Volume
   - Name: "PostProcessVolume"
   - Mode: Global
   - Add Profile

3. **Add Effects**
   - **Vignette**: Darken edges (intensity: 0.3-0.5)
   - **Color Grading**: Slightly desaturate, adjust contrast
   - **Film Grain**: Add slight grain for atmosphere
   - **Bloom**: Subtle bloom on lights (intensity: 0.2-0.3)

### Camera Static Effect

For camera static when animatronics are nearby:

1. Create a noise texture or use shader
2. Apply to StaticOverlay UI element
3. Animate opacity/UV offset for flickering effect
4. Enable when `CameraSystem` detects animatronic nearby

---

## Quick Visual Checklist

- [ ] Office floor and walls created
- [ ] Desk positioned correctly
- [ ] Hallways created (left and right)
- [ ] Main camera at eye level (Y = 1.6)
- [ ] 11 security cameras created with RenderTextures
- [ ] CameraSystem connected to all cameras
- [ ] Left and right doors created
- [ ] DoorSystem connected to door objects
- [ ] Left and right hallway lights created
- [ ] LightSystem connected to lights
- [ ] Animatronic GameObjects created (Bonnie, Chica, Freddy, Foxy)
- [ ] AI scripts attached to animatronics
- [ ] UI Canvas created with power bar, time, night display
- [ ] Camera tablet UI created
- [ ] Game Over and Night Complete panels created
- [ ] Materials created and applied
- [ ] Scene lighting set up (dim and atmospheric)
- [ ] Post-processing effects added (optional)

---

## Tips for Better Visuals

1. **Start Simple**: Use basic shapes (cubes, capsules) first, then replace with models later
2. **Use ProBuilder** (Unity Package): For easier 3D modeling within Unity
3. **Import Free Models**: Use sites like Sketchfab or Unity Asset Store for office furniture
4. **Atmosphere is Key**: Dim lighting and dark colors create the right mood
5. **Performance**: Keep polygon counts reasonable, especially for cameras
6. **Test Often**: Build out one system at a time and test before moving on

---

## Next Steps

Once visuals are set up:

1. Test all systems with the visual setup
2. Adjust lighting and materials for atmosphere
3. Add animations for doors and lights
4. Create or import better models for animatronics
5. Add sound effects and ambient audio
6. Polish UI elements and transitions

This visual setup will transform your game from a blue screen into a fully functional FNAF experience!
