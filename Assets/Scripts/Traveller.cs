using System.Collections;
using UnityEngine;

public class Traveller : BaseMonoBehaviour
{
    [SerializeField]
    private int bpm = 115;

    [SerializeField]
    private float moveDuration = 0.05f;

    [SerializeField]
    private Vector2I initialCoords;

    [SerializeField]
    private TravellerFlasher flasher;

    [SerializeField]
    private TravellerFlasher mainFlasher;

    private PathDrawer pathDrawer = new PathDrawer();

    private Tile currentTile;

    protected override void Awake()
    {
        base.Awake();

        this.currentTile = Board.Instance.GetTile(this.initialCoords);
        this.pathDrawer.AllowedStartingTile = this.currentTile;
    }

    private IEnumerator Start()
    {   
        this.UpdatePosition();

        var beatDelay = this.bpm / 60f / 4f;
        var numBeat = 0;

        while (true)
        {
            if (numBeat % 4 == 0)
            {
                this.mainFlasher.Flash();
            }
            else
            {
                this.flasher.Flash();
            }

            this.StepMovement();

            numBeat++;

            yield return new WaitForSeconds(beatDelay);
        }
    }

    private void Update()
    {
        this.pathDrawer.CheckForInput();
    }

    private IEnumerator LerpToNextTile(Tile nextTile)
    {
        var startTime = Time.time;

        var prevTile = this.currentTile;
        this.currentTile = nextTile;
        this.pathDrawer.AllowedStartingTile = this.currentTile;

        while (true)
        {
            var t = (Time.time - startTime) / this.moveDuration;
            this.transform.position = Vector3.Lerp(prevTile.transform.position, this.currentTile.transform.position, t);

            if (t >= 1f)
            {
                break;
            }

            yield return null;
        }

        this.UpdatePosition();
    }

    private void UpdatePosition()
    {
        this.transform.position = this.currentTile.transform.position;
    }

    private void StepMovement()
    {
        var nextTile = this.pathDrawer.PopFirstTile();

        if (nextTile == null)
        {
            return;
        }

        this.StartCoroutine(this.LerpToNextTile(nextTile));
    }
}
