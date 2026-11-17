// Сердце: не бьёт, а лечит на stats.amount (Heart_Stats / HeartPickup_Stats)
public class Heart : Hazard
{
    protected override void OnHitPlayer(Health player)
    {
        player.Heal(stats.amount);   // НОВОЕ: было Heal(1)
    }
}
