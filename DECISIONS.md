# Decisions log - Eggscape

## Week 10 - Three enemy types (branch feature/enemy-ai)

2026-11-10  Asked Claude (chat) for three enemy FSMs on a new NavMesh arena at the end of Level 2: horse Patroller, llama Shooter, pig Charger. Player3D damages anything tagged "Enemy" on contact (an attack with no wind-up), so the new enemies are Untagged and hurt only after their own wind-up. Sight is one shared EnemySight: distance -> angle -> Raycast with a Walls-only mask.
2026-11-10  Bug: the llama was throwing tomatoes. The Spit prefab was a copy of Tomato and kept the tomato model - replaced its mesh with a green sphere (SpitGreen) and checked that ShooterAI uses Spit, not Tomato.
2026-11-10  Bug: death explosions stayed in the scene forever. EnemyHealth spawned the Explosion prefab but never removed it - added Destroy(fx, 2f) (effectLifeTime field).
2026-11-10  Claude's animation steps assumed Pig.fbx has Walk and Run clips; it only has Idle and Jump. Made Walk and Run states from the Jump clip (speed 0.6 / 1.5) in a new PigEnemyAnim. New *EnemyAnim controllers, so the old horse's HorseAnim is not touched.
2026-11-10  Playtest: the egg starts right in front of the arena so testers meet the enemies at once. Aidana 8 tries, Temirlan 4 tries.