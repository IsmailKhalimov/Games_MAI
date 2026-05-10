using UnityEngine;

public class CrateInteractable : DeferredInteractableBase
{
    [SerializeField] private Rigidbody crateBody;
    [SerializeField] private Vector3 pushDirection = new(1f, 0.3f, 1f);
    [SerializeField] private float pushForce = 6f;
    [SerializeField] private float upwardBias = 0.25f;

    protected override void Awake()
    {
        base.Awake();
        if (crateBody == null)
        {
            crateBody = GetComponent<Rigidbody>();
        }
    }

    public override void ExecuteDeferredAction(DeferredMarkContext context)
    {
        if (crateBody == null)
        {
            return;
        }

        Vector3 direction;
        if (context.HasMarkerPosition)
        {
            Vector3 horizontal = transform.position - context.MarkerWorldPosition;
            horizontal.y = 0f;
            if (horizontal.sqrMagnitude > 0.0001f)
            {
                direction = (horizontal.normalized + Vector3.up * upwardBias).normalized;
            }
            else
            {
                direction = pushDirection.normalized;
            }
        }
        else
        {
            direction = pushDirection.normalized;
        }

        crateBody.AddForce(direction * pushForce, ForceMode.Impulse);
    }

    public void Configure(Rigidbody body, Vector3 direction, float force)
    {
        crateBody = body;
        pushDirection = direction;
        pushForce = force;
    }
}
