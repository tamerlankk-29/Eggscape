EGGSCAPE — 3D mobile platformer (Midterm Project)
Author: Kazhmukan Tamerlan
Course: Game Development Fundamentals, Narxoz University
Engine: Unity 6000.6.0f1, Universal Render Pipeline 17.6, Input System 1.20.0

ABOUT
An egg escapes from a farm. The hero is an egg with little arms and legs:
at low speed it walks on its legs, at high speed it tucks them in and rolls
(physics-based rolling). Collect all coins on each level to open the finish,
avoid hazards, and defeat the cow boss on Level 3.

GAME FLOW
Main Menu -> Level 1 -> Level 2 -> Level 3 (Boss) -> Win -> Main Menu
- Falling off a level or losing all 3 HP restarts the current level.
- The finish flag opens only after all coins on the level are collected.

CONTROLS
On-screen joystick / WASD / arrows — move (slow = walk, fast = roll)
JUMP button / Space — jump
FIRE button / X / Left Mouse — shoot

LEVELS
Level 1 — tutorial: platforms, coins, spikes, falling objects
Level 2 — harder: longer jumps, narrow plank, patrolling horse, moving platform
Level 3 — boss arena: the cow throws fans of tomatoes, warns before each attack,
          gets faster in phase 2 (below half HP); HP bar on screen

HOW TO OPEN
1. Open the project folder in Unity Hub with Unity 6000.6.0f1.
2. Open Assets/Scenes/main menu.unity and press Play.
Scenes in Build Profiles: main menu, Level1, Level2, Level3, Win.

PHONE DEMO
iPhone Air via Unity Remote 5 (Windows PC; an iOS build is not possible without a Mac).
Unity Remote sends touches only to the old input system, so the UI EventSystem uses
StandaloneInputModule. The on-screen stick and buttons still feed the Input System
actions (PlayerControls: Move, Jump, Fire) through PlayerInput.

MAIN SCRIPTS (Assets/Script)
Player3D    — movement relative to camera, jump, coins, finish, enemy damage, moving platform
EggVisual   — walk/roll visual switch, hides arms and legs while rolling
Health      — HP, invincibility coroutine with blinking, restart on death
Shooter     — shooting with cooldown
Bullet      — projectile, hits boss and hazards
Boss        — boss AI: attack coroutine, telegraph, phase 2, HP bar, win
Hazard      — base class for hazards (Rock, Heart, BombHazard inherit from it)
Spawner     — falling objects above the player
CameraFollow, MovingPlatform, Patrol, DamageZone, Hud, MenuButtons, ProfileSettings

ASSETS (sources and licenses)
- Farm Animals Animated — Quaternius — CC0 — https://quaternius.itch.io/lowpoly-animated-animals
- Platformer Kit — Kenney — CC0 — https://kenney.nl/assets/platformer-kit
- Nature Kit — Kenney — CC0 — https://kenney.nl/assets/nature-kit
- Food Kit — Kenney — CC0 — https://kenney.nl/assets/food-kit
- Music "Fun on the Farm" — geoffharvey — Pixabay Content License — https://pixabay.com/music/
- Music "Side Saddle" — geoffharvey — Pixabay Content License — https://pixabay.com/music/
- Sound effects "Jump 6", "Pickup_coin 4" — made by me in jfxr (https://jfxr.frozenfractal.com)
- Sound effects hurt, heal, shoot — made by me (previous assignments)
- Egg hero — egg model from Kenney Food Kit + arms, legs and eyes built from Unity primitives;
  Idle / Run / Jump animations made by me in the Unity Animation window

AI USAGE
AI (Claude) was used for planning, explanations, debugging and part of the code.
Details are in AI_LOG.txt.