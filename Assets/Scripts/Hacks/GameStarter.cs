using UnityEngine;

public class GameStarter : MonoBehaviour
{
	[SerializeField]
	private Song song;

	private void Start()
	{
		var player = new SongPlayer(this.song);
		this.GetComponent<MusicManager>().AssignPlayer(player);
		player.Play();
	}
}
