using UnityEngine;

public interface IDeferredInteractable
{
    string DisplayName { get; }
    Transform MarkerAnchor { get; }
    void OnMarkStateChanged(bool isMarked);
    void ExecuteDeferredAction(DeferredMarkContext context);
}
