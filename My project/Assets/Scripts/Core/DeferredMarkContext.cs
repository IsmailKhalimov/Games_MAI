using UnityEngine;

public readonly struct DeferredMarkContext
{
    public static DeferredMarkContext Default => new(Vector3.zero, false);

    public Vector3 MarkerWorldPosition { get; }
    public bool HasMarkerPosition { get; }

    public DeferredMarkContext(Vector3 markerWorldPosition, bool hasMarkerPosition = true)
    {
        MarkerWorldPosition = markerWorldPosition;
        HasMarkerPosition = hasMarkerPosition;
    }
}
