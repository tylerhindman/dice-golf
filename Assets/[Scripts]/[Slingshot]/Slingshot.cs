//using System.Collections;
//using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Slingshot : MonoBehaviour
{
    public GameObject target;
    public float lineMaxLength;
    public float lineMouseSlingshotScale;

    private Vector3 mouseRotReference;
    // Start is called before the first frame update
    private LineRenderer lineRenderer;
    void Start()
    {
        this.lineRenderer = this.GetComponent<LineRenderer>();
        this.lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) {
            this.lineRenderer.SetPosition(0, new Vector3(this.lineRenderer.GetPosition(0).x, this.lineRenderer.GetPosition(0).y, this.lineRenderer.GetPosition(0).z - 0.1f));
        }

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
                var newArrowAngle = Mathf.Rad2Deg * Mathf.Atan2(mouseDiff.normalized.y, mouseDiff.normalized.x);
                transform.rotation = Quaternion.AngleAxis(newArrowAngle - 90f, Vector3.up);
                transform.position = target.transform.position + Quaternion.AngleAxis(newArrowAngle, Vector3.up) * new Vector3(1.5f, 0, 0);

                var lineLength = Mathf.Clamp((-mouseDiff.magnitude * this.lineMouseSlingshotScale) + this.lineRenderer.GetPosition(1).z, -this.lineMaxLength, this.lineRenderer.GetPosition(1).z);
                this.lineRenderer.SetPosition(0, new Vector3(this.lineRenderer.GetPosition(0).x, this.lineRenderer.GetPosition(0).y, lineLength));
            }
        }

        // Click released
        if (Input.GetMouseButtonUp(0)) {
            this.lineRenderer.enabled = false;
        }
    }
}
