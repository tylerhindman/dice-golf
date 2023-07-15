using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForce : MonoBehaviour
{
    Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        _rb.AddTorque(UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(0, 50));
        _rb.AddForce(UnityEngine.Random.Range(-25, 25), 0, UnityEngine.Random.Range(10, 25));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
