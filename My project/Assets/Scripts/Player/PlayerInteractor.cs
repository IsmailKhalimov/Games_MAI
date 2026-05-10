using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSensitivity = 120f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float interactDistance = 4f;

    private CharacterController controller;
    private float verticalVelocity;
    private float pitch;
    private IDeferredInteractable currentTarget;

    public event Action<string> OnTargetLabelChanged;
    public event Action<string> OnStatusChanged;

    public void Configure(Camera cam, float speed, float look, float distance)
    {
        playerCamera = cam;
        moveSpeed = speed;
        lookSensitivity = look;
        interactDistance = distance;
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        UpdateCurrentTarget();
        HandleInputActions();
    }

    private void HandleMovement()
    {
        Vector2 input = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;
        }

        Vector3 move = (transform.right * input.x + transform.forward * input.y).normalized * moveSpeed;
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }

    private void HandleLook()
    {
        if (Mouse.current == null || playerCamera == null)
        {
            return;
        }

        Vector2 delta = Mouse.current.delta.ReadValue() * (lookSensitivity * Time.deltaTime);
        pitch -= delta.y;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up * delta.x);
    }

    private void UpdateCurrentTarget()
    {
        currentTarget = null;
        if (playerCamera == null)
        {
            OnTargetLabelChanged?.Invoke("Цель: нет");
            return;
        }

        var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            OnTargetLabelChanged?.Invoke("Цель: нет");
            return;
        }

        DeferredInteractableBase interactable = hit.collider.GetComponentInParent<DeferredInteractableBase>();
        if (interactable == null)
        {
            OnTargetLabelChanged?.Invoke("Цель: нет");
            return;
        }

        currentTarget = interactable;
        OnTargetLabelChanged?.Invoke($"Цель: {interactable.DisplayName}");
    }

    private void HandleInputActions()
    {
        if (Keyboard.current == null || DeferredActionManager.Instance == null)
        {
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            var markContext = new DeferredMarkContext(transform.position);
            bool success = DeferredActionManager.Instance.TryMark(currentTarget, markContext);
            if (success)
            {
                OnStatusChanged?.Invoke("Метка поставлена/обновлена");
            }
        }

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            bool success = DeferredActionManager.Instance.TriggerAllMarks();
            OnStatusChanged?.Invoke(success ? "Запуск отложенных действий" : "Нет меток для активации");
        }
    }
}
