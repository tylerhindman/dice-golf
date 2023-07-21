using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[Serializable]
public class Slingshot : MonoBehaviour
{
    public GameObject diceTarget;
    public float lineAnchorOffset;
    public float lineMaxLength;
    public float maxMouseY;
    public float forceMultiplier;
    public float forceHeightOffset;
    public float velocityStopThreshold;
    public float slingshotLineTrackingSpeed;

    private GameManager gameManager;
    private DiceStateMachine stateMachine;
    private PlayerInfo playerInfo;
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    private PlayerInput slingshotControlManager;
    private InputActionMap slingshotControls;
    private bool rollDelayFlag = false;
    private bool slingshotHeld = false;
    private float mouseYSum;
    private string playerDevice;

    void Start()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
        this.lineRenderer.enabled = false;

        this.gameManager = FindObjectOfType<GameManager>();
        this.stateMachine = this.diceTarget.GetComponent<DiceStateMachine>();

        this.playerInfo = gameObject.transform.parent.GetComponentInChildren<PlayerInfo>();
        this.slingshotControlManager = this.GetComponent<PlayerInput>();
        this.slingshotControls = this.slingshotControlManager.currentActionMap;
        // Use this later to manually assign controllers??
        // this.slingshotControlManager.SwitchCurrentControlScheme();

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
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("New Input system Y Delta: " + this.slingshotControls.FindAction("Slingshot Pull").ReadValue<float>());
        if (this.stateMachine.state == DiceStateMachine.State.Slingshot) {
            // Mouse held
            if (this.slingshotHeld) {
                // Get mouse diff vector vals
                var mouseY = this.slingshotControls.FindAction("Slingshot Pull").ReadValue<float>();
                // Mouse
                if (this.playerDevice == "Mouse") {
                    mouseY /= this.maxMouseY;
                    this.mouseYSum = Mathf.Clamp(this.mouseYSum + mouseY, -1f, 0f);
                // Gamepad
                } else {
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
                }
            }
        } else if (this.stateMachine.state == DiceStateMachine.State.Rolling && this.rollDelayFlag) {
            if (this.diceTarget.GetComponent<Rigidbody>().velocity.magnitude <= this.velocityStopThreshold) {
                this.stateMachine.setNextState(DiceStateMachine.State.Resolve);
            }
        }
    }

    public void registerCamera(Camera camera) {
        this.mainCamera = camera;
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
        }
    }

    void slingshotButtonReleased() {
        if (this.stateMachine.state == DiceStateMachine.State.Slingshot && this.slingshotHeld) {
            // Hide line renderer after click release
            this.lineRenderer.enabled = false;
            this.slingshotHeld = false;

            // Apply force based on slingshot magnitude
            if (this.mouseYSum < 0f) {
                this.diceTarget.GetComponent<Rigidbody>().AddForceAtPosition(this.mainCamera.transform.forward * Mathf.Clamp(-this.mouseYSum, 0f, 1f) * this.forceMultiplier,
                    new Vector3(this.diceTarget.transform.position.x, this.diceTarget.transform.position.y + this.forceHeightOffset, this.diceTarget.transform.position.z),
                    ForceMode.Impulse);
                this.stateMachine.setNextState(DiceStateMachine.State.Rolling);
                StartCoroutine(rollDelay());
            }
        }
    }
}
