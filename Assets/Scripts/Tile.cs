using System.Collections.Generic;
using UnityEngine;

public class Tile : BaseMonoBehaviour
{
    [SerializeField]
    private float flipSpeed = 90f;

    private Flip currentFlip;
    private Vector2 beforeRotation;
    private Vector2 afterRotation;

    private Face currentFace;
    private Vector2 nextFaceRotation = Vector2.zero;

    private Queue<Flip> flips = new Queue<Flip>();
    private Queue<Face> backFaces = new Queue<Face>();

    private void Update()
    {
        var percentToRotate = this.flipSpeed * Time.smoothDeltaTime;

        while (percentToRotate > 0f)
        {
            var shouldFlip = this.CheckCurrentFlip();

            if (!shouldFlip)
            {
                break;
            }

            var progressChange = Mathf.Min(1f - this.currentFlip.Progress, percentToRotate);
            percentToRotate -= progressChange;
            this.currentFlip.Progress += progressChange;

            var newRotation = Vector2.Lerp(this.beforeRotation, this.afterRotation, this.currentFlip.Progress);
            this.UpdateRotation(newRotation);
        }
    }

    private void UpdateRotation(Vector2 rotation)
    {
        this.transform.localEulerAngles = rotation;
    }

    private bool CheckCurrentFlip()
    {
        if (this.currentFlip == null)
        {
            if (this.flips.Count > 0)
            {
                this.currentFlip = this.flips.Dequeue();
                this.OnStartFlip(this.currentFlip);
                this.UpdateRotation(this.beforeRotation);
            }
        }

        if (this.currentFlip == null)
        {
            return false;
        }

        if (this.currentFlip.Progress < 1f)
        {
            return true;
        }

        this.OnEndFlip(this.currentFlip);
        this.currentFlip = null;
        return this.CheckCurrentFlip();
    }

    private void OnStartFlip(Flip flip)
    {
        this.beforeRotation = this.afterRotation;
        this.afterRotation += flip.Type.ToOffset();

        if (this.nextFaceRotation == Vector2.zero)
        {
            this.nextFaceRotation = Vector2.up * 180;
        }
        else
        {
            this.nextFaceRotation = Vector2.zero;
        }

        flip.TargetFace.transform.localEulerAngles = this.nextFaceRotation;
        flip.TargetFace.gameObject.SetActive(true);
    }

    private void OnEndFlip(Flip flip)
    {
        if (flip.DestroyPreviousFace)
        {
            flip.PreviousFace.Destroy();
        }
        else
        {
            flip.PreviousFace.gameObject.SetActive(false);
        }
    }

    public void SetPosition(Vector2 position)
    {
        this.transform.localPosition = position;
    }

    public void AddFace(Face face)
    {
        Debug.Assert(face != null, this);

        face.transform.SetParent(this.transform);
        GameObjectUtility.ResetLocalTransform(face.transform, true);

        if (this.currentFace == null)
        {
            this.currentFace = face;
            this.currentFace.gameObject.SetActive(true);
        }
        else
        {
            this.backFaces.Enqueue(face);
            face.gameObject.SetActive(false);

            if (this.backFaces.Count == 1)
            {
                this.backFaces.Enqueue(this.currentFace);
            }
        }
    }

    public void Flip(bool destroyCurrentFace)
    {
        var targetFace = this.backFaces.Count > 0 ? this.backFaces.Dequeue() : null;
        var previousFace = this.currentFace;

        if (!destroyCurrentFace)
        {
            this.backFaces.Enqueue(targetFace);
        }

        var flip = new Flip
        {
            Type = EnumUtility.RandomValue<FlipType>(),
            TargetFace = targetFace,
            PreviousFace = previousFace,
            DestroyPreviousFace = destroyCurrentFace,
        };

        this.currentFace = targetFace;

        this.flips.Enqueue(flip);
    }
}
