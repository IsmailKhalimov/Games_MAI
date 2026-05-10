using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeferredActionManager : MonoBehaviour
{
    public static DeferredActionManager Instance { get; private set; }

    [SerializeField] private float activationDelaySeconds = 1f;

    private readonly Dictionary<IDeferredInteractable, DeferredMarkContext> marks = new();
    private Coroutine activationRoutine;

    public event Action<int> OnMarkCountChanged;
    public event Action<float> OnCountdownChanged;
    public event Action<bool> OnActivationStateChanged;

    public int MarkCount => marks.Count;
    public bool IsActivating => activationRoutine != null;
    public float ActivationDelaySeconds => activationDelaySeconds;

    public void SetActivationDelay(float seconds)
    {
        activationDelaySeconds = Mathf.Max(0.1f, seconds);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool TryMark(IDeferredInteractable interactable, DeferredMarkContext context)
    {
        if (interactable == null || IsActivating)
        {
            return false;
        }

        if (marks.ContainsKey(interactable))
        {
            // Перезаписываем метку на том же объекте по правилам задания.
            marks[interactable] = context;
            interactable.OnMarkStateChanged(true);
            return true;
        }

        marks.Add(interactable, context);
        interactable.OnMarkStateChanged(true);
        OnMarkCountChanged?.Invoke(marks.Count);
        return true;
    }

    public bool TriggerAllMarks()
    {
        if (IsActivating || marks.Count == 0)
        {
            return false;
        }

        activationRoutine = StartCoroutine(ActivateRoutine());
        return true;
    }

    private IEnumerator ActivateRoutine()
    {
        OnActivationStateChanged?.Invoke(true);
        float remaining = activationDelaySeconds;
        OnCountdownChanged?.Invoke(remaining);

        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;
            OnCountdownChanged?.Invoke(Mathf.Max(remaining, 0f));
            yield return null;
        }

        var toExecute = new List<KeyValuePair<IDeferredInteractable, DeferredMarkContext>>(marks);
        marks.Clear();
        OnMarkCountChanged?.Invoke(0);

        foreach (KeyValuePair<IDeferredInteractable, DeferredMarkContext> markedAction in toExecute)
        {
            IDeferredInteractable interactable = markedAction.Key;
            if (interactable == null)
            {
                continue;
            }

            interactable.OnMarkStateChanged(false);
            interactable.ExecuteDeferredAction(markedAction.Value);
        }

        OnCountdownChanged?.Invoke(0f);
        activationRoutine = null;
        OnActivationStateChanged?.Invoke(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
