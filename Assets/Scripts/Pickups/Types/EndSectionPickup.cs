public class EndSectionPickup : Pickup
{
    protected override void OnPickup()
    {
        base.OnPickup();

        MusicManager.Instance.Player.EndSection();
    }
}
