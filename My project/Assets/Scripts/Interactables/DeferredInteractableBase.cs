using UnityEngine;

public abstract class DeferredInteractableBase : MonoBehaviour, IDeferredInteractable
{
    [SerializeField] private string displayName = "Interactable";
    [SerializeField] private Renderer markerRenderer;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color markedColor = Color.yellow;
    [SerializeField] private Transform markerAnchor;

    private Material runtimeMaterial;

    public string DisplayName => displayName;
    public Transform MarkerAnchor => markerAnchor != null ? markerAnchor : transform;

    public void SetDisplayName(string newName)
    {
        displayName = newName;
    }

    public void SetMarkerRenderer(Renderer renderer)
    {
        markerRenderer = renderer;
    }

    public void SetColors(Color baseColor, Color activeColor)
    {
        defaultColor = baseColor;
        markedColor = activeColor;

        if (runtimeMaterial != null)
        {
            runtimeMaterial.color = defaultColor;
        }
    }

    protected virtual void Awake()
    {
        if (markerRenderer == null)
        {
            markerRenderer = GetComponentInChildren<Renderer>();
        }

        if (markerRenderer != null)
        {
            runtimeMaterial = markerRenderer.material;
            runtimeMaterial.color = defaultColor;
        }
    }

    public virtual void OnMarkStateChanged(bool isMarked)
    {
        if (runtimeMaterial == null)
        {
            return;
        }

        runtimeMaterial.color = isMarked ? markedColor : defaultColor;
    }

    public abstract void ExecuteDeferredAction(DeferredMarkContext context);
}
