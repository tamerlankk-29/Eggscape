# Eggscape

A 3D mobile platformer made in Unity 6 (6000.6.0f1, URP).
A brave egg escapes from a farm: it walks on tiny legs, tucks them in to roll,
collects coins to open the finish and defeats the cow boss.

- Platform: Android, landscape
- Author: Kazhmukan Tamerlan, Game Development Fundamentals, Narxoz University
- Game design document: Docs/Eggscape - GDD v1.pdf

## How to open
1. Open this folder in Unity Hub with Unity 6000.6.0f1.
2. Open Assets/Scenes/main menu.unity and press Play.

## Controls
Joystick: move · JUMP: jump · FIRE: shoot · II: pause

Asset sources and licenses are listed in README.txt. AI usage is logged in AI_LOG.txt.
## Playtest notes (by Temirlan)
- Rolling feels fun, but long jumps on Level 2 are hard with the joystick
- Pause button is easy to reach; JUMP and FIRE are close, maybe move FIRE higher
- Idea for the cut list: no skins shop, focus on levels

## Sound and music (sources and licenses)

| File | What it is | Source | License |
|---|---|---|---|
| Sounds/Jump 6.wav | egg jump | made by me in jfxr (https://jfxr.frozenfractal.com) | own work |
| Sounds/Pickup_coin 4.wav | coin pickup | made by me in jfxr | own work |
| Sounds/hurt.wav | egg takes damage | made by me in jfxr | own work |
| Sounds/heal.wav | heart pickup | made by me in jfxr | own work |
| Sounds/shoot.wav | egg shot, llama spit | made by me in jfxr | own work |
| Sounds/horse_windup.wav | horse wind-up before the kick | made by me in jfxr | own work |
| Sounds/llama_aim.wav | llama aims before spitting | made by me in jfxr | own work |
| Sounds/pig_windup.wav | pig wind-up before the charge | made by me in jfxr | own work |
| Sounds/Explosion 7.wav | pig charge hit | made by me in jfxr | own work |
| Sounds/boss_windup.wav | cow telegraph before the tomato fan | made by me in jfxr | own work |
| Sounds/death.wav | egg dies (0 HP) | made by me in jfxr | own work |
| Sounds/win.wav | level complete | made by me in jfxr | own work |
| Sounds/lose.wav | level restart after a fall or death | made by me in jfxr | own work |
| Sounds/ui_click.wav | every UI button | made by me in jfxr | own work |
| Sounds/Music/music_menu.ogg | menu and Win music | "Side Saddle" by geoffharvey, Pixabay, https://pixabay.com/music/upbeat-side-saddle-427912/ (converted MP3 -> OGG) | Pixabay Content License |
| Sounds/Music/music_level.ogg | Level 1-3 music | "Fun on the Farm" by geoffharvey, Pixabay, https://pixabay.com/music/cartoons-fun-on-the-farm-377651/ (converted MP3 -> OGG) | Pixabay Content License |

Mixer: Assets/Audio/Main.mixer — Master > Music > Tracks, Master > SFX, Master > UI.
Exposed: MasterVol, MusicVol, SFXVol. Snapshots: Normal, Paused (Tracks -6 dB, Lowpass 800 Hz).
