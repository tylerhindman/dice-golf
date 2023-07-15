//using System.Collections;
//using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Slingshot : MonoBehaviour
{
    public GameObject diceTarget;
    public float lineAnchorOffset;
    public float lineMaxLength;
    public float lineMouseSlingshotScale;
    public float maxMouseY;

    private Vector3 mouseRotReference;
    // Start is called before the first frame update
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    void Start()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
        this.lineRenderer.enabled = false;

        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Grab mouse reference point on left click
        if (Input.GetMouseButtonDown(0)) {
            this.mouseRotReference = Input.mousePosition;
        }

        // When left click is held, rotate per reference
        if (Input.GetMouseButton(0)) {
            var x = -(Input.mousePosition.x - this.mouseRotReference.x);
            var y = (Input.mousePosition.y - this.mouseRotReference.y);
            var mouseDiff = new Vector3(x, y, 0);
            if (x != 0 || y != 0) {
                if (!this.lineRenderer.enabled) {
                    this.lineRenderer.enabled = true;
                }
                var newArrowAngle = Mathf.Rad2Deg * Mathf.Atan2(mainCamera.transform.forward.x, mainCamera.transform.forward.z);
                //var newArrowAngle = Mathf.Rad2Deg * Mathf.Atan2(mouseDiff.normalized.y, mouseDiff.normalized.x);
                //newArrowAngle -= cameraForwardAngleFromTarget;

                transform.rotation = Quaternion.AngleAxis(newArrowAngle - 180f, Vector3.up);
                transform.position = this.diceTarget.transform.position + Quaternion.AngleAxis(newArrowAngle - 90f, Vector3.up) * new Vector3(this.lineAnchorOffset, 0, 0);

                var lineLength = Mathf.Lerp(this.lineRenderer.GetPosition(1).z, -this.lineMaxLength, -y / this.maxMouseY);
                //var lineLength = Mathf.Clamp((-mouseDiff.magnitude * this.lineMouseSlingshotScale) + this.lineRenderer.GetPosition(1).z, -this.lineMaxLength, this.lineRenderer.GetPosition(1).z);
                this.lineRenderer.SetPosition(0, new Vector3(this.lineRenderer.GetPosition(0).x, this.lineRenderer.GetPosition(0).y, lineLength));

                Debug.Log("mouseY: " + y + "percentage: " + -y / this.maxMouseY);
            }
        }

        // Click released
        if (Input.GetMouseButtonUp(0)) {
            this.lineRenderer.enabled = false;
        }
    }
}
