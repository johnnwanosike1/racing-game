using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Feeds player input (keyboard + optional on-screen buttons) into CarController.
/// Works with the generated CarInput actions.
/// </summary>
[RequireComponent(typeof(CarController))]
public class PlayerCarInput : MonoBehaviour
{
    [Header("Smoothing")]
    public float dampenSpeed = 6f;
    public AnimationCurve steeringCurve = AnimationCurve.Linear(0f, 1f, 120f, 1f);

    [Header("Optional Mobile/UI Buttons")]
    public Mybutton gasPedal;
    public Mybutton brakePedal;
    public Mybutton leftButton;
    public Mybutton rightButton;

    private CarInput input;
    private CarController carController;

    private float throttleRaw;
    private float steerRaw;
    private float clutchRaw;
    private float handbrakeRaw;

    private float throttleSmoothed;
    private float steerSmoothed;
    private float clutchSmoothed;

    private void Awake()
    {
        input = new CarInput();
        carController = GetComponent<CarController>();
    }

    private void OnEnable()
    {
        input.Enable();

        input.Car.Throttle.performed += OnThrottle;
        input.Car.Throttle.canceled += OnThrottle;

        input.Car.Steering.performed += OnSteer;
        input.Car.Steering.canceled += OnSteer;

        input.Car.Clutch.performed += OnClutch;
        input.Car.Clutch.canceled += OnClutch;

        input.Car.Handbrake.performed += OnHandbrake;
        input.Car.Handbrake.canceled += OnHandbrake;
    }

    private void OnDisable()
    {
        input.Car.Throttle.performed -= OnThrottle;
        input.Car.Throttle.canceled -= OnThrottle;

        input.Car.Steering.performed -= OnSteer;
        input.Car.Steering.canceled -= OnSteer;

        input.Car.Clutch.performed -= OnClutch;
        input.Car.Clutch.canceled -= OnClutch;

        input.Car.Handbrake.performed -= OnHandbrake;
        input.Car.Handbrake.canceled -= OnHandbrake;

        input.Disable();
    }

    private void Update()
    {
        ApplyUiButtons();

        throttleSmoothed = Mathf.Lerp(throttleSmoothed, throttleRaw, Time.deltaTime * dampenSpeed);
        steerSmoothed = Mathf.Lerp(steerSmoothed, steerRaw, Time.deltaTime * dampenSpeed);
        clutchSmoothed = Mathf.Lerp(clutchSmoothed, clutchRaw, Time.deltaTime * dampenSpeed);

        float steerScaled = steerSmoothed;
        if (steeringCurve != null)
            steerScaled *= steeringCurve.Evaluate(carController.speed);

        carController.SetInput(throttleSmoothed, steerScaled, clutchSmoothed, handbrakeRaw);
    }

    private void ApplyUiButtons()
    {
        // Build a clean input state each frame (no accumulation).
        float uiThrottle = 0f;
        float uiSteer = 0f;

        if (gasPedal != null && gasPedal.isPressed)
            uiThrottle += gasPedal.dampenPress;
        if (brakePedal != null && brakePedal.isPressed)
            uiThrottle -= brakePedal.dampenPress;

        if (rightButton != null && rightButton.isPressed)
            uiSteer += rightButton.dampenPress;
        if (leftButton != null && leftButton.isPressed)
            uiSteer -= leftButton.dampenPress;

        // If UI is being used, override keyboard axes (prevents fight between inputs).
        if (Mathf.Abs(uiThrottle) > 0.001f) throttleRaw = Mathf.Clamp(uiThrottle, -1f, 1f);
        if (Mathf.Abs(uiSteer) > 0.001f) steerRaw = Mathf.Clamp(uiSteer, -1f, 1f);
    }

    private void OnThrottle(InputAction.CallbackContext ctx)
    {
        throttleRaw = Mathf.Clamp(ctx.ReadValue<float>(), -1f, 1f);
    }

    private void OnSteer(InputAction.CallbackContext ctx)
    {
        steerRaw = Mathf.Clamp(ctx.ReadValue<float>(), -1f, 1f);
    }

    private void OnClutch(InputAction.CallbackContext ctx)
    {
        clutchRaw = Mathf.Clamp01(ctx.ReadValue<float>());
    }

    private void OnHandbrake(InputAction.CallbackContext ctx)
    {
        handbrakeRaw = Mathf.Clamp01(ctx.ReadValue<float>());
    }
}
