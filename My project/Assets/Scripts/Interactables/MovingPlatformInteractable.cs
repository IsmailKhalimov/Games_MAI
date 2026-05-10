using UnityEngine;

public class MovingPlatformInteractable : DeferredInteractableBase
{
    [SerializeField] private Vector3 moveOffset = new(0f, 0f, 6f);
    [SerializeField] private float moveSpeed = 2f;

    private Vector3 startPosition;
    private bool isMoving;
    private float movementTime;

    public void Configure(Vector3 offset, float speed)
    {
        moveOffset = offset;
        moveSpeed = speed;
        startPosition = transform.position;
    }

    protected override void Awake()
    {
        base.Awake();
        startPosition = transform.position;
    }

    private void Update()
    {
        if (!isMoving)
        {
            return;
        }

        movementTime += Time.deltaTime;
        float t = (Mathf.Sin(movementTime * moveSpeed) + 1f) * 0.5f;
        transform.position = Vector3.Lerp(startPosition, startPosition + moveOffset, t);
    }

    public override void ExecuteDeferredAction(DeferredMarkContext context)
    {
        isMoving = !isMoving;
    }
}
