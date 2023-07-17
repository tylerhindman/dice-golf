using System;
using System.Collections;
using UnityEngine;

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

    private float mouseYReference;
    private GameManager gameManager;
    private DiceStateMachine stateMachine;
    private PlayerInfo playerInfo;
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    private bool rollDelayFlag = false;
    //private bool slingshotHeld = false;
    void Start()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
        this.lineRenderer.enabled = false;

        this.gameManager = FindObjectOfType<GameManager>();
        this.stateMachine = this.diceTarget.GetComponent<DiceStateMachine>();

        this.playerInfo = gameObject.transform.parent.GetComponentInChildren<PlayerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.stateMachine.state == DiceStateMachine.State.Slingshot) {
            //Debug.Log("Player 1 button: " + Input.GetAxis(this.playerInfo.playerButtonAxis) + ", Player 1 stick: " + Input.GetAxis(this.playerInfo.playerStickAxis));

            // Mouse down trigger
            if (Input.GetMouseButtonDown(0)) {
            //if (!this.slingshotHeld && Input.GetAxis(this.playerInfo.playerButtonAxis) == 1) {
                // Grab mouse reference point on left click
                this.mouseYReference = 0;
                this.rollDelayFlag = false;
                //this.slingshotHeld = true;
            }

            // Mouse held
            if (Input.GetMouseButton(0)) {
            //if (this.slingshotHeld) {
                // Get mouse diff vector vals
                var mouseY = Input.GetAxis("Mouse Y");
                //var mouseY = Input.GetAxis(this.playerInfo.playerStickAxis);
                this.mouseYReference += mouseY;
                // Only update slingshot if mouse moved from reference
                if (this.mouseYReference < 0f) {
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
                    var lineLength = Mathf.Lerp(this.lineRenderer.GetPosition(1).z, -this.lineMaxLength, -this.mouseYReference / this.maxMouseY);
                    this.lineRenderer.SetPosition(0, new Vector3(this.lineRenderer.GetPosition(0).x, this.lineRenderer.GetPosition(0).y, lineLength));
                }
            }

            // Click released trigger
            if (Input.GetMouseButtonUp(0)) {
            //if (this.slingshotHeld && Input.GetAxis(this.playerInfo.playerButtonAxis) == 0) {
                // Hide line renderer after click release
                this.lineRenderer.enabled = false;
                //this.slingshotHeld = false;

                // Get mouse diff vector vals
                var mouseY = Input.GetAxis("Mouse Y");
                // Apply force based on slingshot magnitude
                if (this.mouseYReference < 0f) {
                    this.diceTarget.GetComponent<Rigidbody>().AddForceAtPosition(this.mainCamera.transform.forward * Mathf.Clamp(-this.mouseYReference / this.maxMouseY, 0f, 1f) * this.forceMultiplier,
                        new Vector3(this.diceTarget.transform.position.x, this.diceTarget.transform.position.y + this.forceHeightOffset, this.diceTarget.transform.position.z),
                        ForceMode.Impulse);
                    this.stateMachine.setNextState(DiceStateMachine.State.Rolling);
                    StartCoroutine(rollDelay());
                }
            }
        } else if (this.stateMachine.state == DiceStateMachine.State.Rolling && this.rollDelayFlag) {
            Debug.Log("velocityMag: " + this.diceTarget.GetComponent<Rigidbody>().velocity.magnitude);
            if (this.diceTarget.GetComponent<Rigidbody>().velocity.magnitude <= this.velocityStopThreshold) {
                this.gameManager.IncreaseRollCount();
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
}
