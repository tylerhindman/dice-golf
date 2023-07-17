using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RayDice : MonoBehaviour
{
    public event Action<int, Color> DiceValueReturned = delegate { };

    Rigidbody _rb;
    MeshRenderer _mr;

    bool _hasLanded;
    bool _thrown;

    Vector3 initPosition;

    protected int _diceValue;
    public int DiceValue => _diceValue;

    [SerializeField] ParticleSystem _resolveParticles;
    [SerializeField] GameObject _resolvePopup;
    //[SerializeField] AudioClip _diceResolveSFX;
    //[SerializeField] AudioClip _diceThrowSFX;
    //[SerializeField] AudioClip _diceHitSFX;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    [SerializeField] private GameObject sphere;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mr = GetComponent<MeshRenderer>();
        initPosition = transform.position;
        _rb.useGravity = false;

        // Get the MeshFilter component and its shared mesh
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter component not found!");
            return;
        }

        mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            Debug.LogError("Mesh not found!");
            return;
        }

        // Extract vertices and triangles from the mesh
        vertices = mesh.vertices;
        triangles = mesh.triangles;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            RollDice();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //AudioManager.Instance.PlaySound2D(_diceHitSFX, .15f, UnityEngine.Random.Range(.8f, 1.2f));
    }

    private void FixedUpdate()
    {
        if (_rb.IsSleeping() && !_hasLanded && _thrown)
        {
            _hasLanded = true;
            _rb.isKinematic = true;
            _rb.useGravity = false;
            GetDiceValue(Raycast());
        }
        // If the dice stopped moving and is stuck, reroll the dice
        else if (_rb.IsSleeping() && _hasLanded && _diceValue == -1)
        {
            RollAgain();
            Debug.Log("AHHHHHHHHHHHHHHHHHHHHH");
        }
    }

    public void RollDice()
    {
        // Set rigidbody bools to allow movement, add force + torque
        // Will not roll if the dice is in the process of being thrown or already landed
        if (!_thrown && !_hasLanded)
        {
            _thrown = true;
            _rb.isKinematic = false;
            _rb.useGravity = true;
            _rb.AddTorque(UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(0, 50));
            _rb.AddForce(UnityEngine.Random.Range(-25, 25), 0, UnityEngine.Random.Range(10, 25));

            //AudioManager.Instance.PlaySound2D(_diceThrowSFX, .5f, UnityEngine.Random.Range(.8f, 1.2f));
        }
        else if (_thrown && _hasLanded)
        {
            DiceReset();
        }
    }

    // Resets all transform and rigidbody values and bools of the dice
    void DiceReset()
    {
        transform.position = initPosition;
        transform.rotation = Quaternion.identity;
        _rb.isKinematic = true;
        _thrown = false;
        _hasLanded = false;
        _rb.useGravity = false;
    }

    // Exact same as RollDice(), but runs DiceReset() first. Makes the reroll automatic for stuck dice
    void RollAgain()
    {
        DiceReset();
        _thrown = true;
        _rb.isKinematic = false;
        _rb.useGravity = true;
        _rb.AddTorque(UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(0, 50));
        _rb.AddForce(UnityEngine.Random.Range(-25, 25), 0, UnityEngine.Random.Range(10, 25));

        //AudioManager.Instance.PlaySound2D(_diceThrowSFX, .5f, UnityEngine.Random.Range(.8f, 1.2f));
    }

    // Testing different directions to set a value to the dice. Will return -1 if stuck

    // Could possibly make new dice with different values as long as I replace the dots on the dice relative to the new values
    public int GetDiceValue(int value)
    {
        //Debug.Log("eulerAngles: " + dieRotation.x + ", " + dieRotation.y + ", " + dieRotation.z + " Value: " + iValue);

        _diceValue = value;

        DiceValueReturned?.Invoke(_diceValue, _mr.material.color);

        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);

        ParticleSystem particles = Instantiate(_resolveParticles, transform.position, Quaternion.identity);

        ParticleSystem.MainModule settings = particles.main;
        settings.startColor = new ParticleSystem.MinMaxGradient(_mr.material.color);

        ScorePopup popup = Instantiate(_resolvePopup, pos, Quaternion.identity).GetComponent<ScorePopup>();

        popup.transform.rotation = Quaternion.Euler(45, 0, 0);

        popup.text.text = ("[<color=#" + ColorUtility.ToHtmlStringRGB(_mr.material.color) + ">" + value + "</color>]");
        popup.text.color = Color.white;
        popup.text.outlineColor = Color.black;

        //AudioManager.Instance.PlaySound2D(_diceResolveSFX, .5f, UnityEngine.Random.Range(.8f,1.2f));

        return (value);
    }

    // If dice is out of bounds, reroll
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "FallZone")
        {
            RollAgain();
        }
    }

    private int Raycast()
    {
      int shortestDistanceIndex = -1;
      float shortestDistance = 1000;

        // Perform a raycast from each face -- Find faces by getting vertices and creating triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {

            //Debug.Log(vertices.Length);

            // D6 and D10 because they have 2 triangles per face
            if (triangles.Length == 36 || (triangles.Length == 60 && vertices.Length == 40))
            {
                // Get the vertices of the current face
                Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
                Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
                Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);
                Vector3 v3 = transform.TransformPoint(vertices[triangles[i + 3]]);
                Vector3 v4 = transform.TransformPoint(vertices[triangles[i + 4]]);
                Vector3 v5 = transform.TransformPoint(vertices[triangles[i + 5]]);

                // Calculate the face normal
                Vector3 faceNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
                //Vector3 faceNormal2 = Vector3.Cross(v4 - v3, v5 - v3).normalized;

                // Perform a raycast from the face centroid along the face normal
                Vector3 faceCentroid = (v0 + v1 + v2 + v3 + v4 + v5) / 6f;

                GameObject test = Instantiate(sphere, faceCentroid, Quaternion.identity);
                test.name = ("Side" + ((i / 6f) + 1));
                test.transform.LookAt(faceCentroid + faceNormal * 50);
                //test.transform.rotation = Quaternion.Euler(test.transform.rotation.x, test.transform.rotation.y, test.transform.rotation.z);

                Debug.DrawRay(faceCentroid - (faceNormal * 0.1f), faceNormal * 3, Color.red, 50f, true);

                RaycastHit hit;
                if (Physics.Raycast(faceCentroid - (faceNormal * 0.1f), faceNormal * 20, out hit))
                {
                    if (hit.collider.tag == "Ground")
                    {
                        float distanceToGround = hit.distance;

                        // Check if the current distance is shorter than the shortest distance found so far
                        if (distanceToGround < shortestDistance)
                        {
                            shortestDistance = distanceToGround;

                            if (i == 0)
                            {
                                Debug.Log("i: " + i + " --- " + "index: " + 1);
                                shortestDistanceIndex = 1; // Store the index of the raycast
                            }
                            else
                            {
                                Debug.Log("i: " + i + " --- " + "index: " + (float)((i / 6f) + 1));
                                shortestDistanceIndex = (int)((i / 6f) + 1); // Store the index of the raycast
                            }
                        }
                    }
                }
                i += 3;
            }
            // D12 because it has 3 triangles per face
            else if (triangles.Length == 108)
            {
                // Get the vertices of the current face
                Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
                Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
                Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);
                Vector3 v3 = transform.TransformPoint(vertices[triangles[i + 3]]);
                Vector3 v4 = transform.TransformPoint(vertices[triangles[i + 4]]);
                Vector3 v5 = transform.TransformPoint(vertices[triangles[i + 5]]);
                Vector3 v6 = transform.TransformPoint(vertices[triangles[i + 6]]);
                Vector3 v7 = transform.TransformPoint(vertices[triangles[i + 7]]);
                Vector3 v8 = transform.TransformPoint(vertices[triangles[i + 8]]);

                // Calculate the face normal
                Vector3 faceNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

                // Perform a raycast from the face centroid along the face normal
                Vector3 faceCentroid = (v0 + v1 + v2 + v3 + v4 + v5 + v6 + v7 + v8) / 9f;

                GameObject test = Instantiate(sphere, faceCentroid, Quaternion.identity);
                test.name = ("Side" + ((i / 9f) + 1));
                test.transform.LookAt(faceCentroid + faceNormal * 50);
                //Quaternion.LookRotation(faceCentroid, faceNormal);

                Debug.DrawRay(faceCentroid - (faceNormal * 0.1f), faceNormal * 3, Color.red, 50f, true);

                RaycastHit hit;
                if (Physics.Raycast(faceCentroid - (faceNormal * 0.1f), faceNormal * 3, out hit))
                {
                    if (hit.collider.tag == "Ground")
                    {
                        float distanceToGround = hit.distance;

                        // Check if the current distance is shorter than the shortest distance found so far
                        if (distanceToGround < shortestDistance)
                        {
                            shortestDistance = distanceToGround;
                            if (i == 0)
                            {
                                Debug.Log("i: " + i + " --- " + "index: " + 1);
                                shortestDistanceIndex = 1; // Store the index of the raycast
                            }
                            else
                            {
                                Debug.Log("i: " + i + " --- " + "index: " + (float)((i / 9f) + 1));
                                shortestDistanceIndex = (int)((i / 9f) + 1); // Store the index of the raycast
                            }
                        }
                    }
                }
                i += 6;
            }
            else
            {
                // Get the vertices of the current face
                Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
                Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
                Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

                // Calculate the face normal
                Vector3 faceNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

                // Perform a raycast from the face centroid along the face normal
                Vector3 faceCentroid = (v0 + v1 + v2) / 3f;

                GameObject test = Instantiate(sphere, faceCentroid, Quaternion.identity);
                test.name = ("Side" + ((i / 3f) + 1));
                test.transform.LookAt(faceCentroid + faceNormal * 50);
                //Quaternion.LookRotation(faceCentroid, faceNormal);

                Debug.DrawRay(faceCentroid - (faceNormal * 0.1f), faceNormal * 3, Color.red, 50f, true);

                RaycastHit hit;
                if (Physics.Raycast(faceCentroid - (faceNormal * 0.1f), faceNormal * 3, out hit))
                {
                    if (hit.collider.tag == "Ground")
                    {
                        float distanceToGround = hit.distance;

                        // Check if the current distance is shorter than the shortest distance found so far
                        if (distanceToGround < shortestDistance)
                        {
                            shortestDistance = distanceToGround;

                            if (i == 0)
                            {
                                Debug.Log("i: " + i + " --- " + "index: " + 1);
                                shortestDistanceIndex = 1; // Store the index of the raycast
                            }
                            else
                            {
                                Debug.Log("i: " + i + " --- " + "index: " + (float)((i / 3f) + 1));
                                shortestDistanceIndex = ((i / 3) + 1); // Store the index of the raycast
                            }
                            
                        }
                    }
                }

            }
        }
        
        return shortestDistanceIndex;

    }
}

