//using System.Collections;
//using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Slingshot : MonoBehaviour
{
    public GameObject slingshotPrefab;

    private Vector3 mouseRotReference;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            var lScale = this.GetComponent<Transform>().localScale;
            this.GetComponent<Transform>().localScale = new Vector3(lScale.x + 0.1f, lScale.y, lScale.z);
        }

        // Grab mouse reference point on left click
        if (Input.GetMouseButtonDown(0)) {
            this.mouseRotReference = Input.mousePosition;
        }

        // When left click is held, rotate per reference
        if (Input.GetMouseButton(0)) {
            var newArrowAngle = Mathf.Tan((Input.mousePosition.x - this.mouseRotReference.x) / (Input.mousePosition.y - this.mouseRotReference.y));

            this.gameObject.transform.eulerAngles = new Vector3(
                this.gameObject.transform.eulerAngles.x,
                newArrowAngle,
                this.gameObject.transform.eulerAngles.z
            );
        }
    }
}
