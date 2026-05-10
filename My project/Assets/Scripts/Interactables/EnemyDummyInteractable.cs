using UnityEngine;

public class EnemyDummyInteractable : DeferredInteractableBase
{
    [SerializeField] private int maxHealth = 64;
    [SerializeField] private int damagePerActivation = 16;
    [SerializeField] private Renderer bodyRenderer;
    [SerializeField] private Color damagedColor = Color.red;
    [SerializeField] private Vector3 popupOffset = new(0f, 2.2f, 0f);
    [SerializeField] private float popupDuration = 0.8f;
    [SerializeField] private float popupRiseDistance = 1f;
    [SerializeField] private float kneelDuration = 0.35f;
    [SerializeField] private float fallDuration = 0.45f;
    [SerializeField] private float kneelDepth = 0.5f;
    [SerializeField] private float finalFallAngle = 85f;

    private int currentHealth;
    private Material bodyMaterial;
    private Color initialColor;
    private bool isDead;
    private bool deathAnimationStarted;

    protected override void Awake()
    {
        base.Awake();
        currentHealth = maxHealth;

        if (bodyRenderer == null)
        {
            bodyRenderer = GetComponentInChildren<Renderer>();
        }

        if (bodyRenderer != null)
        {
            bodyMaterial = bodyRenderer.material;
            initialColor = bodyMaterial.color;
        }
    }

    public override void ExecuteDeferredAction(DeferredMarkContext context)
    {
        if (isDead)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damagePerActivation);
        ShowDamagePopup();

        if (bodyMaterial != null)
        {
            bodyMaterial.color = currentHealth > 0 ? damagedColor : Color.black;
        }

        if (currentHealth == 0)
        {
            isDead = true;
            if (!deathAnimationStarted)
            {
                deathAnimationStarted = true;
                StartCoroutine(PlayDeathAnimation());
            }
        }
        else if (bodyMaterial != null)
        {
            Invoke(nameof(RestoreColor), 0.25f);
        }
    }

    private void RestoreColor()
    {
        if (bodyMaterial != null && currentHealth > 0)
        {
            bodyMaterial.color = initialColor;
        }
    }

    public void ConfigureDamage(int damage)
    {
        damagePerActivation = Mathf.Max(1, damage);
    }

    private void ShowDamagePopup()
    {
        GameObject popupObject = new("DamagePopup");
        popupObject.transform.position = transform.position + popupOffset;

        TextMesh textMesh = popupObject.AddComponent<TextMesh>();
        textMesh.text = $"-{damagePerActivation}";
        textMesh.fontSize = 48;
        textMesh.characterSize = 0.05f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = new Color(1f, 0.2f, 0.2f, 1f);

        StartCoroutine(AnimatePopup(popupObject.transform, textMesh));
    }

    private System.Collections.IEnumerator AnimatePopup(Transform popupTransform, TextMesh textMesh)
    {
        Vector3 startPos = popupTransform.position;
        Vector3 endPos = startPos + Vector3.up * popupRiseDistance;
        float elapsed = 0f;

        while (elapsed < popupDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / popupDuration);
            popupTransform.position = Vector3.Lerp(startPos, endPos, t);

            if (Camera.main != null)
            {
                Vector3 cameraDirection = popupTransform.position - Camera.main.transform.position;
                popupTransform.rotation = Quaternion.LookRotation(cameraDirection.normalized);
            }

            Color color = textMesh.color;
            color.a = 1f - t;
            textMesh.color = color;
            yield return null;
        }

        Destroy(popupTransform.gameObject);
    }

    private System.Collections.IEnumerator PlayDeathAnimation()
    {
        Vector3 kneelStartPos = transform.localPosition;
        Quaternion kneelStartRot = transform.localRotation;
        Vector3 kneelTargetPos = kneelStartPos + Vector3.down * kneelDepth;
        Quaternion kneelTargetRot = kneelStartRot * Quaternion.Euler(18f, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < kneelDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / kneelDuration);
            transform.localPosition = Vector3.Lerp(kneelStartPos, kneelTargetPos, t);
            transform.localRotation = Quaternion.Slerp(kneelStartRot, kneelTargetRot, t);
            yield return null;
        }

        Vector3 fallStartPos = transform.localPosition;
        Quaternion fallStartRot = transform.localRotation;
        Quaternion fallTargetRot = fallStartRot * Quaternion.Euler(finalFallAngle, 0f, 0f);

        elapsed = 0f;
        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fallDuration);
            transform.localPosition = Vector3.Lerp(fallStartPos, fallStartPos + Vector3.down * 0.1f, t);
            transform.localRotation = Quaternion.Slerp(fallStartRot, fallTargetRot, t);
            yield return null;
        }
    }
}
