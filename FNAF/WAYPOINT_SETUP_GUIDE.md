# Waypoint Setup Guide

A simple guide for setting up waypoints for animatronics in your FNAF game.

## Quick Setup

### 1. Create LocationManager (if not present)

- Unity will auto-create a `LocationManager` when needed, or you can manually create one:
  - Right-click in Hierarchy → Create Empty
  - Name it "LocationManager"
  - Add Component → Location Manager

### 2. Create Waypoints

For each location where animatronics should move:

1. **Create Empty GameObject**

   - Right-click in Hierarchy → Create Empty
   - Name it descriptively (e.g., "Waypoint_ShowStage", "Waypoint_DiningArea")
   - Position it in the Scene view where the animatronic should stand

2. **Add LocationWaypoint Component**

   - Select the GameObject
   - Add Component → Location Waypoint

3. **Configure Waypoint Settings**

   - **Waypoint Name**: Set a unique name (e.g., "ShowStage", "DiningArea", "WestHall")
   - **Allow All Animatronics**:
     - ✅ Checked = All animatronics can use this waypoint
     - ❌ Unchecked = Only specific animatronics can use it
   - **Allowed Animatronic Names**: (Only if "Allow All" is unchecked)
     - Set Size to number of animatronics
     - Enter animatronic names (e.g., "Bonnie", "Freddy", "Chica", "Foxy")

4. **Optional Visual Settings**
   - **Gizmo Color**: Color of waypoint sphere in Scene view
   - **Show Label**: Display waypoint name in Scene view
   - **Gizmo Size**: Size of waypoint sphere

### 3. Configure Animatronics

For each animatronic (Bonnie, Freddy, etc.):

1. **Select Animatronic GameObject**
2. **Set Animatronic Name** (must match waypoint filter names)
   - In Inspector, find the AI script (e.g., "Bonnie AI")
   - Set "Animatronic Name" field (e.g., "Bonnie")
3. **Set AI Level** (0-20)
   - 0 = No movement
   - Higher = More frequent movement

## How It Works

- **Waypoint Discovery**: LocationManager automatically finds all waypoints on Start
- **Movement**: Animatronics with `aiLevel > 0` will automatically move between waypoints
- **Occupancy**: Only one animatronic can occupy a waypoint at a time
- **Teleportation**: Animatronics instantly teleport to waypoints (no smooth movement)

## Game Over Trigger

When an animatronic enters a waypoint named **"Office"** (case-insensitive), the game automatically:

1. **Triggers Jumpscare**: The jumpscare system activates (camera shake, visual effects, etc.)
2. **Game Over**: The game state changes to `GameOver`
3. **Game Over Screen**: A game over screen is displayed with restart options

### Setting Up the Office Waypoint

To create a game over trigger:

1. **Create an Office Waypoint**
   - Create an Empty GameObject named "Waypoint_Office"
   - Position it where the office entrance should be
   - Add the `LocationWaypoint` component

2. **Configure the Waypoint**
   - Set **Waypoint Name** to exactly **"Office"** (this name is checked automatically)
   - Configure which animatronics can reach it (e.g., allow "Bonnie", "Chica", etc.)
   - **Important**: The waypoint name must be "Office" (case-insensitive) for the game over to trigger

3. **Add to Animatronic Path**
   - Include "Office" in your animatronic's movement path (e.g., in `BonnieAI.cs`)
   - The animatronic will automatically trigger game over when it reaches this waypoint

### Example

```csharp
// In BonnieAI.cs, the path includes "Office" as the final destination
string[] bonniePath = new string[]
{
    "ShowStage",
    "DiningArea",
    "WestHall",
    "WestHallCorner",
    "SupplyCloset",
    "LeftDoor",
    "Office"  // ← This triggers game over when reached
};
```

**Note**: The game over check happens automatically when an animatronic occupies any waypoint named "Office". No additional setup required!

## Testing

- Press **K** in Play Mode to manually cycle Bonnie between waypoints
- Check the **Current Waypoint Name** field in the Inspector to see where each animatronic is located

## Tips

- **Waypoint Names**: Use descriptive names that match your animatronic's movement paths
- **Bonnie's Path**: Bonnie looks for waypoints named: "ShowStage", "DiningArea", "WestHall", "WestHallCorner", "SupplyCloset", "LeftDoor", "Office"
- **Office Waypoint**: Any waypoint named "Office" will automatically trigger game over when an animatronic enters it
- **Universal Waypoints**: Check "Allow All Animatronics" for shared locations like hallways
- **Visual Feedback**: Enable "Show Label" to see waypoint names in Scene view while editing

## Troubleshooting

**Animatronic not moving?**

- Check `aiLevel > 0` in Inspector
- Verify waypoint allows the animatronic (check "Allow All" or add animatronic name to list)
- Ensure LocationManager exists in scene

**Waypoint not found?**

- Check waypoint name matches what animatronic is looking for
- Verify waypoint is in the scene (not a prefab)
- Check console for LocationManager discovery logs

**Animatronic stuck?**

- Waypoint may be occupied by another animatronic
- Check "Current Waypoint Name" field to see current location
- Use K key in QuickTest to manually cycle waypoints
