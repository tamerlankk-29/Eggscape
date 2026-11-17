using UnityEngine;

// Опасности и предметы. По ассету на каждый префаб: Assets/Data/Hazards/...
[CreateAssetMenu(fileName = "Hazard_Stats", menuName = "Eggscape/Hazard Stats")]
public class HazardStats : ScriptableObject
{
    public int amount = 1;         // урон игроку; у сердца - сколько лечит
    public float lifeTime = 6f;    // с до исчезновения; 0 = не исчезает
    public float fuseTime = 0f;    // только бомба: с от падения до взрыва
}
