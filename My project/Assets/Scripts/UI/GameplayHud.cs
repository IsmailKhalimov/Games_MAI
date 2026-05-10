using UnityEngine;
using UnityEngine.UI;

public class GameplayHud : MonoBehaviour
{
    [SerializeField] private Text targetText;
    [SerializeField] private Text marksText;
    [SerializeField] private Text countdownText;
    [SerializeField] private Text hintText;
    [SerializeField] private Text statusText;
    [SerializeField] private PlayerInteractor playerInteractor;

    public void Configure(Text target, Text marks, Text countdown, Text hint, Text status, PlayerInteractor interactor)
    {
        targetText = target;
        marksText = marks;
        countdownText = countdown;
        hintText = hint;
        statusText = status;
        playerInteractor = interactor;
    }

    private void Awake()
    {
        if (targetText != null) targetText.text = "Цель: нет";
        if (marksText != null) marksText.text = "Метки: 0";
        if (countdownText != null) countdownText.text = "Задержка: -";
        if (hintText != null) hintText.text = "E - пометить, F - активировать все";
        if (statusText != null) statusText.text = "Статус: ожидание";
    }

    private void Start()
    {
        if (playerInteractor != null)
        {
            playerInteractor.OnTargetLabelChanged += HandleTargetChanged;
            playerInteractor.OnStatusChanged += HandleStatusChanged;
        }

        if (DeferredActionManager.Instance != null)
        {
            DeferredActionManager.Instance.OnMarkCountChanged += HandleMarkCountChanged;
            DeferredActionManager.Instance.OnCountdownChanged += HandleCountdownChanged;
            DeferredActionManager.Instance.OnActivationStateChanged += HandleActivationStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (playerInteractor != null)
        {
            playerInteractor.OnTargetLabelChanged -= HandleTargetChanged;
            playerInteractor.OnStatusChanged -= HandleStatusChanged;
        }

        if (DeferredActionManager.Instance != null)
        {
            DeferredActionManager.Instance.OnMarkCountChanged -= HandleMarkCountChanged;
            DeferredActionManager.Instance.OnCountdownChanged -= HandleCountdownChanged;
            DeferredActionManager.Instance.OnActivationStateChanged -= HandleActivationStateChanged;
        }
    }

    private void HandleTargetChanged(string targetLabel)
    {
        if (targetText != null)
        {
            targetText.text = targetLabel;
        }
    }

    private void HandleMarkCountChanged(int count)
    {
        if (marksText != null)
        {
            marksText.text = $"Метки: {count}";
        }
    }

    private void HandleCountdownChanged(float seconds)
    {
        if (countdownText == null)
        {
            return;
        }

        countdownText.text = seconds > 0f ? $"Задержка: {seconds:0.0}с" : "Задержка: -";
    }

    private void HandleActivationStateChanged(bool active)
    {
        if (statusText != null)
        {
            statusText.text = active ? "Статус: активация..." : "Статус: ожидание";
        }
    }

    private void HandleStatusChanged(string message)
    {
        if (statusText != null)
        {
            statusText.text = $"Статус: {message}";
        }
    }
}
