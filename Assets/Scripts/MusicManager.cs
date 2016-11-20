public class MusicManager : BehaviourSingleton<MusicManager>
{
    public SongPlayer Player { get; private set; }

    public void AssignPlayer(SongPlayer player)
    {
        this.Player = player;
    }
}
