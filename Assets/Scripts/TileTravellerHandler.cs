using UnityEngine;

public class TileTravellerHandler : BaseMonoBehaviour
{
    [SerializeField]
    private Tile tile;

    private bool isActivated;

    private void OnTriggerStay(Collider other)
    {
        if (this.isActivated)
        {
            return;
        }

        this.isActivated = true;
        this.tile.Backing.Highlight(true);
    }
}
