using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[Serializable]
public class Slingshot : MonoBehaviour
{
    [Header("Gameobject Props")]
    public GameObject diceTarget;
    [Header("Visuals/Controls")]
    public float lineAnchorOffset;
    public float lineMaxLength;
    public float maxMouseY;

    // Object refs
    private GameManager gameManager;
    private DiceStateMachine stateMachine;
    private PlayerInfo playerInfo;
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    private PlayerInput slingshotControlManager;
    private InputActionMap slingshotControls;
    private DiceModelSwap diceModelSwapComponent;
    private Rigidbody rb;
    // Flags
    private bool rollDelayFlag = false;
    private bool slingshotHeld = false;
    // Slingshot controls
    private float mouseYSum;
    private string playerDevice;
    // Animation
    private float diceTargetStartingY;
    private Vector3 randomRotation;

    // Props set from DiceProperties
    private float forceMultiplier;
    private float forceHeightOffsetBase;
    private float forceHeightOffsetMax;
    private float velocityStopThreshold;
    private float slingshotRisingAnimationEndPercentage;
    private float slingshotRisingAnimationMaxHeight;
    private float slingshotRollingAnimationStartPercentage;
    private float slingshotRollingAnimationMaxSpeed;


    void Start()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
        this.lineRenderer.enabled = false;

        this.gameManager = FindObjectOfType<GameManager>();
        this.stateMachine = this.diceTarget.GetComponent<DiceStateMachine>();

        this.playerInfo = gameObject.transform.parent.GetComponentInChildren<PlayerInfo>();
        this.slingshotControlManager = this.GetComponent<PlayerInput>();
        this.rb = this.diceTarget.GetComponent<Rigidbody>();
        this.slingshotControls = this.slingshotControlManager.currentActionMap;
        // Use this later to manually assign controllers??
        // this.slingshotControlManager.SwitchCurrentControlScheme();

        this.diceModelSwapComponent = gameObject.transform.parent.GetComponentInChildren<DiceModelSwap>();

        // Slingshot button pressed
        this.slingshotControls.FindAction("Slingshot Pull Button").performed += ctx => {
            this.playerDevice = ctx.control.device.ToString().Contains("Mouse") ? "Mouse" : "Gamepad";
            var button = (ButtonControl)ctx.control;
            if (button.isPressed) {
                this.slingshotButtonPressed();
            }
        };

        // Slingshot button unpressed
        this.slingshotControls.FindAction("Slingshot Pull Button").canceled += ctx => {
            var button = (ButtonControl)ctx.control;
            if (!button.isPressed) {
                this.slingshotButtonReleased();
            }
        };

        // Dice Model Swap left button pressed
        this.slingshotControls.FindAction("Dice Model Swap - Left").performed += ctx => {
            if (this.gameManager.debugDiceSwapEnabled && this.diceModelSwapComponent != null) {
                this.diceModelSwapComponent.swapLeft();
            }
        };

        // Dice Model Swap right button pressed
        this.slingshotControls.FindAction("Dice Model Swap - Right").performed += ctx => {
            if (this.gameManager.debugDiceSwapEnabled && this.diceModelSwapComponent != null) {
                this.diceModelSwapComponent.swapRight();
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (this.stateMachine.state == DiceStateMachine.State.Slingshot) {
            // Mouse held
            if (this.slingshotHeld) {
                // Get mouse diff vector vals
                var mouseY = this.slingshotControls.FindAction("Slingshot Pull").ReadValue<float>();
                var mouseYDiffThisFrame = 0.0f;
                // Mouse
                if (this.playerDevice == "Mouse") {
                    mouseY /= this.maxMouseY;
                    mouseYDiffThisFrame = Mathf.Clamp(-mouseY, -1.0f, 1.0f);
                    this.mouseYSum = Mathf.Clamp(this.mouseYSum + mouseY, -1f, 0f);
                // Gamepad
                } else {
                    mouseYDiffThisFrame = Mathf.Clamp((-this.mouseYSum) + mouseY, -1.0f, 1.0f);
                    this.mouseYSum = Mathf.Clamp(mouseY, -1f, 0f);
                }
                // Only update slingshot if mouse moved from reference
                if (this.mouseYSum <= 0f) {
                    // Enable line renderer first time
                    if (!this.lineRenderer.enabled) {
                        this.lineRenderer.enabled = true;
                    }
                    // Get new angle to rotate too based on main camera forward
                    var newArrowAngle = Mathf.Rad2Deg * Mathf.Atan2(mainCamera.transform.forward.x, mainCamera.transform.forward.z);

                    // Rotate slingshot based on new angle
                    transform.rotation = Quaternion.AngleAxis(newArrowAngle - 180f, Vector3.up);
                    transform.position = this.diceTarget.transform.position + Quaternion.AngleAxis(newArrowAngle - 90f, Vector3.up) * new Vector3(this.lineAnchorOffset, 0, 0);

                    // Update length of slingshot based on mouse y
                    var lineLength = Mathf.Lerp(this.lineRenderer.GetPosition(1).z, -this.lineMaxLength, -this.mouseYSum);
                    this.lineRenderer.SetPosition(0, new Vector3(this.lineRenderer.GetPosition(0).x, this.lineRenderer.GetPosition(0).y, lineLength));

                    if (this.gameManager.debugDiceSpinAnimationEnabled) {
                        // Rising animation
                        var newRisingPos = new Vector3(
                            this.rb.position.x,
                            Mathf.Lerp(this.diceTargetStartingY, this.diceTargetStartingY + this.slingshotRisingAnimationMaxHeight,
                                Mathf.Clamp(-this.mouseYSum / this.slingshotRisingAnimationEndPercentage, 0.0f, 1.0f)
                            ),
                            this.rb.position.z
                        );
                        this.diceTarget.transform.position = newRisingPos;
                        this.rb.position = newRisingPos;

                        // Rotating animation
                        if (mouseYDiffThisFrame != 0.0f && this.mouseYSum != 0.0f) {
                            this.rb.AddRelativeTorque(new Vector3(
                                    this.randomRotation.x * mouseYDiffThisFrame,
                                    this.randomRotation.y * mouseYDiffThisFrame,
                                    this.randomRotation.z * mouseYDiffThisFrame
                                ),
                                ForceMode.VelocityChange
                            );
                        }
                    }
                }

                // Debug draw camera ray
                Debug.DrawRay(this.rb.position, new Vector3(this.mainCamera.transform.forward.x,
                        this.forceHeightOffsetBase + Mathf.Lerp(0.0f, this.forceHeightOffsetMax, Mathf.Clamp(1.0f - (-this.mouseYSum), 0.0f, 1.0f)), // The less force, the greater angle up
                        this.mainCamera.transform.forward.z), Color.red, 0.3f, true);
            }
        } else if (this.stateMachine.state == DiceStateMachine.State.Rolling && this.rollDelayFlag) {
            if (this.rb.velocity.magnitude <= this.velocityStopThreshold) {
                this.stateMachine.setNextState(DiceStateMachine.State.Resolve);
            }
        }
    }

    public void registerCamera(Camera camera) {
        this.mainCamera = camera;
    }

    public void setDiceProperties(DicePropertiesScriptableObject diceProperties) {
        this.forceMultiplier = diceProperties.forceMultiplier;
        this.forceHeightOffsetBase = diceProperties.forceHeightOffsetBase;
        this.forceHeightOffsetMax = diceProperties.forceHeightOffsetMax;
        this.velocityStopThreshold = diceProperties.velocityStopThreshold;
        this.slingshotRisingAnimationEndPercentage = diceProperties.slingshotRisingAnimationEndPercentage;
        this.slingshotRisingAnimationMaxHeight = diceProperties.slingshotRisingAnimationMaxHeight;
        this.slingshotRollingAnimationStartPercentage = diceProperties.slingshotRollingAnimationStartPercentage;
        this.slingshotRollingAnimationMaxSpeed = diceProperties.slingshotRollingAnimationMaxSpeed;
    }

    IEnumerator rollDelay()
    {
        yield return new WaitForSeconds(0.02f);

        this.rollDelayFlag = true;
    }

    // Pocket state trigger
    void PocketState() {

    }

    void ResolveState() {

    }

    void slingshotButtonPressed() {
        if (this.stateMachine.state == DiceStateMachine.State.Slingshot && !this.slingshotHeld) {
            this.slingshotHeld = true;
            this.mouseYSum = 0;
            this.rollDelayFlag = false;
            if (this.gameManager.debugDiceSpinAnimationEnabled) {
                this.diceTargetStartingY = this.rb.position.y;
                this.rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                this.rb.maxAngularVelocity = this.slingshotRollingAnimationMaxSpeed;
                this.rb.useGravity = false; // Turn off gravity because it accumulates while held in the air
                this.setRandomRotation();
            }
        }
    }

    void slingshotButtonReleased() {
        if (this.stateMachine.state == DiceStateMachine.State.Slingshot && this.slingshotHeld) {
            // Hide line renderer after click release
            this.lineRenderer.enabled = false;
            this.slingshotHeld = false;
            if (this.gameManager.debugDiceSpinAnimationEnabled) {
                this.rb.constraints = RigidbodyConstraints.None;
                this.rb.maxAngularVelocity = 7; // default
                this.rb.useGravity = true; // Need this bad boy again
            }

            // Apply force based on slingshot magnitude
            if (this.mouseYSum < 0f) {
                var forceY = 0.0f;
                if (this.gameManager.debugDiceSpinAnimationEnabled) {
                    forceY = this.forceHeightOffsetBase + Mathf.Lerp(0.0f, this.forceHeightOffsetMax, Mathf.Clamp(1.0f - (-this.mouseYSum), 0.0f, 1.0f));
                }
                this.rb.AddForceAtPosition(
                    new Vector3(
                        this.mainCamera.transform.forward.x,
                        forceY, // The less force, the greater angle up
                        this.mainCamera.transform.forward.z
                    ) * Mathf.Clamp(-this.mouseYSum, 0f, 1f) * this.forceMultiplier,
                    new Vector3(this.rb.position.x, this.rb.position.y + (this.gameManager.debugDiceSpinAnimationEnabled ? 0.0f : 0.15f), this.rb.position.z),
                    ForceMode.Impulse);
                this.stateMachine.setNextState(DiceStateMachine.State.Rolling);
                StartCoroutine(rollDelay());
            }
        }
    }

    void setRandomRotation() {
        this.randomRotation = new Vector3(
                UnityEngine.Random.Range(-this.slingshotRollingAnimationMaxSpeed, this.slingshotRollingAnimationMaxSpeed),
                UnityEngine.Random.Range(-this.slingshotRollingAnimationMaxSpeed, this.slingshotRollingAnimationMaxSpeed),
                UnityEngine.Random.Range(-this.slingshotRollingAnimationMaxSpeed, this.slingshotRollingAnimationMaxSpeed)
            );
    }

}
