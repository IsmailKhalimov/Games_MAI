using UnityEngine;

public class DoorInteractable : DeferredInteractableBase
{
    [SerializeField] private Transform doorLeaf;
    [SerializeField] private Vector3 openOffset = new(0f, 3f, 0f);
    [SerializeField] private float openSpeed = 2f;

    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private bool isMoving;
    private bool isOpen;

    public void Configure(Transform leaf, Vector3 offset, float speed)
    {
        doorLeaf = leaf;
        openOffset = offset;
        openSpeed = speed;
        closedPosition = doorLeaf.position;
        targetPosition = closedPosition;
    }

    protected override void Awake()
    {
        base.Awake();
        if (doorLeaf == null)
        {
            doorLeaf = transform;
        }

        closedPosition = doorLeaf.position;
        targetPosition = closedPosition;
    }

    private void Update()
    {
        if (!isMoving)
        {
            return;
        }

        doorLeaf.position = Vector3.MoveTowards(doorLeaf.position, targetPosition, openSpeed * Time.deltaTime);
        if (Vector3.SqrMagnitude(doorLeaf.position - targetPosition) <= 0.0001f)
        {
            doorLeaf.position = targetPosition;
            isMoving = false;
        }
    }

    public override void ExecuteDeferredAction(DeferredMarkContext context)
    {
        isOpen = !isOpen;
        targetPosition = isOpen ? closedPosition + openOffset : closedPosition;
        isMoving = true;
    }
}
