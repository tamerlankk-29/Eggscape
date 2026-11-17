# Decisions log - Eggscape

## Week 10 - Three enemy types (branch feature/enemy-ai)

2026-11-10  Asked Claude (chat) for three enemy FSMs on a new NavMesh arena at the end of Level 2: horse Patroller, llama Shooter, pig Charger. Player3D damages anything tagged "Enemy" on contact (an attack with no wind-up), so the new enemies are Untagged and hurt only after their own wind-up. Sight is one shared EnemySight: distance -> angle -> Raycast with a Walls-only mask.
2026-11-10  Bug: the llama was throwing tomatoes. The Spit prefab was a copy of Tomato and kept the tomato model - replaced its mesh with a green sphere (SpitGreen) and checked that ShooterAI uses Spit, not Tomato.
2026-11-10  Bug: death explosions stayed in the scene forever. EnemyHealth spawned the Explosion prefab but never removed it - added Destroy(fx, 2f) (effectLifeTime field).
2026-11-10  Claude's animation steps assumed Pig.fbx has Walk and Run clips; it only has Idle and Jump. Made Walk and Run states from the Jump clip (speed 0.6 / 1.5) in a new PigEnemyAnim. New *EnemyAnim controllers, so the old horse's HorseAnim is not touched.
2026-11-10  Playtest: the egg starts right in front of the arena so testers meet the enemies at once. Aidana 8 tries, Temirlan 4 tries. ChargerAI: Wind Up 0.7 → 0.8, ShooterAI: Cooldown 1.5 → 2.0, PatrollerAI: Chase Speed 4.5 → 3.5 Now Aidana 4 tries.ы

## Week 11 - Data and saves (branch feature/data-saves)

2026-11-17  Asked Claude (chat) to move balance into data assets and add a JSON save. Every balance number now lives in ScriptableObjects: Player_Stats, Horse/Llama/Pig_Stats, Cow_Stats and 7 HazardStats. Current HP stays on each enemy; the asset keeps only maxHp, otherwise one hit would damage every pig.
2026-11-17  Levels: platforms stay in the three scenes, everything that sets difficulty is in Level_01..03 (goal, time limit, hazard rain, enemy spawns, points). One LevelLoader script reads any of them. Horse waypoints became Vector3 points in Level_02, because an asset cannot reference objects in a scene.
2026-11-17  Save: JsonUtility (not BinaryFormatter) with try/catch, a version field and defaults; written to save.tmp first, then swapped in. Saved after a level, when settings close and in OnApplicationPause. Settings moved from PlayerPrefs into save.json.
2026-11-17  Bug: ArgumentException "The Object you want to instantiate is null" in Spawner. The Hazard Prefabs list in Level_02 had 3 empty slots. Filled it with Bomb, Rock, Heart. Claude's Spawner did not check for empty slots, so one empty field in a data asset crashed the rain.
2026-11-17  Claude wrote the 35 script files into the project; I created the 16 assets, wired prefabs and scenes, built the level select and reset panels.