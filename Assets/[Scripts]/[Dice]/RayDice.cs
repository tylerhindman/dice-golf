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

    // Resolve Actions
    [SerializeField] ParticleSystem _resolveParticles;
    [SerializeField] GameObject _resolvePopup;

    // Audio
    //[SerializeField] AudioClip _diceResolveSFX;
    //[SerializeField] AudioClip _diceThrowSFX;
    //[SerializeField] AudioClip _diceHitSFX;

    // FOR MATH, FACES, AND RAYCASTS
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    // To vizualize location of faces and spawn the plane per face
    [SerializeField] private GameObject sphere;

    // Dice Type Enum
    private enum DiceType
    {
        D4,
        D6,
        D8,
        D10,
        D12,
        D20
    }

    DiceType diceType;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mr = GetComponent<MeshRenderer>();
        //initPosition = transform.position;
        //_rb.useGravity = false;

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

        switch(vertices.Length)
        {
            case 12:
                diceType = DiceType.D4;
                break;

            case 24:
                diceType = DiceType.D6;
                break;

            case 22:
                diceType = DiceType.D8;
                break;

            case 40:
                diceType = DiceType.D10;
                break;

            case 60:
                if(triangles.Length == 108)
                {
                    diceType = DiceType.D12;
                }
                else if(triangles.Length == 60)
                {
                    diceType = DiceType.D20;
                }
                break;
        }

        //Debug.Log(gameObject.name + "   " + diceType);
        //Debug.Log(gameObject.name + "   " + vertices.Length);
        //Debug.Log(gameObject.name + "   " + triangles.Length);
    }

    private void Start()
    {
        //RollDice();
    }

    private void Update()
    {

        /*if (Input.GetKeyDown(KeyCode.R))
        {
            RollDice();
        }*/
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
            //_rb.isKinematic = true;
            //_rb.useGravity = false;
            GetDiceValue(Raycast());
        }
        // If the dice stopped moving and is stuck, reroll the dice
        else if (_rb.IsSleeping() && _hasLanded && _diceValue == -1)
        {
            //RollAgain();
            Debug.Log("ROLL AGAIN");
        }
    }

    public void RollDice()
    {
        // Set rigidbody bools to allow movement, add force + torque
        // Will not roll if the dice is in the process of being thrown or already landed
        if (!_thrown && !_hasLanded)
        {
            _thrown = true;
            //_rb.isKinematic = false;
            //_rb.useGravity = true;
            //_rb.AddTorque(UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(0, 50));
            //_rb.AddForce(UnityEngine.Random.Range(-25, 25), 0, UnityEngine.Random.Range(10, 25));

            //AudioManager.Instance.PlaySound2D(_diceThrowSFX, .5f, UnityEngine.Random.Range(.8f, 1.2f));
        }
        else if (_thrown && _hasLanded)
        {
            Debug.Log("DICE RESET");
            DiceReset();
        }
    }

    // Resets all transform and rigidbody values and bools of the dice
    void DiceReset()
    {
        //transform.position = initPosition;
        //transform.rotation = Quaternion.identity;
        //_rb.isKinematic = true;
        _thrown = false;
        _hasLanded = false;
        //_rb.useGravity = false;
    }

    // Exact same as RollDice(), but runs DiceReset() first. Makes the reroll automatic for stuck dice
/*    void RollAgain()
    {
        DiceReset();
        _thrown = true;
        _rb.isKinematic = false;
        _rb.useGravity = true;
        _rb.AddTorque(UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(0, 50));
        _rb.AddForce(UnityEngine.Random.Range(-25, 25), 0, UnityEngine.Random.Range(10, 25));

        //AudioManager.Instance.PlaySound2D(_diceThrowSFX, .5f, UnityEngine.Random.Range(.8f, 1.2f));
    }*/

    // Testing different directions to set a value to the dice. Will return -1 if stuck

    // Could possibly make new dice with different values as long as I replace the dots on the dice relative to the new values
    public int GetDiceValue(int value)
    {
        //Debug.Log("eulerAngles: " + dieRotation.x + ", " + dieRotation.y + ", " + dieRotation.z + " Value: " + iValue);

        _diceValue = value;

        DiceValueReturned?.Invoke(_diceValue, _mr.material.color);

        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        ParticleSystem particles = Instantiate(_resolveParticles, transform.position, Quaternion.identity);

        ParticleSystem.MainModule settings = particles.main;
        settings.startColor = new ParticleSystem.MinMaxGradient(_mr.material.color);

        ScorePopup popup = Instantiate(_resolvePopup, pos, Quaternion.identity).GetComponent<ScorePopup>();

        popup.transform.LookAt(Camera.main.transform.position);
        popup.transform.Rotate(new Vector3(0,180,0));

        popup.text.text = ("[<color=#" + ColorUtility.ToHtmlStringRGB(_mr.material.color) + ">" + value + "</color>]");
        popup.text.color = Color.white;
        popup.text.outlineColor = Color.black;

        //AudioManager.Instance.PlaySound2D(_diceResolveSFX, .5f, UnityEngine.Random.Range(.8f,1.2f));

        return (value);
    }

    // If dice is out of bounds, reroll
/*    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "FallZone")
        {
            RollAgain();
        }
    }*/

    private int Raycast()
    {
      int shortestDistanceIndex = -1;
      float shortestDistance = 1000;

        // Perform a raycast from each face -- Find faces by getting vertices and creating triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {

            //Debug.Log(vertices.Length);

            // D6 and D10 because they have 2 triangles per face
            if (diceType == DiceType.D6 || diceType == DiceType.D10)
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

                RemoveSphere(test);

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
                                //Debug.Log("i: " + i + " --- " + "index: " + 1);
                                shortestDistanceIndex = 1; // Store the index of the raycast
                            }
                            else
                            {
                                //Debug.Log("i: " + i + " --- " + "index: " + (float)((i / 6f) + 1));
                                shortestDistanceIndex = (int)((i / 6f) + 1); // Store the index of the raycast
                            }
                        }
                    }
                }
                i += 3;
            }
            // D12 because it has 3 triangles per face
            else if (diceType == DiceType.D12)
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
                                //Debug.Log("i: " + i + " --- " + "index: " + 1);
                                shortestDistanceIndex = 1; // Store the index of the raycast
                            }
                            else
                            {
                                //Debug.Log("i: " + i + " --- " + "index: " + (float)((i / 9f) + 1));
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
                                //Debug.Log("i: " + i + " --- " + "index: " + 1);
                                shortestDistanceIndex = 1; // Store the index of the raycast
                            }
                            else
                            {
                                //Debug.Log("i: " + i + " --- " + "index: " + (float)((i / 3f) + 1));
                                shortestDistanceIndex = ((i / 3) + 1); // Store the index of the raycast
                            }
                            
                        }
                    }
                }

            }
        }

        //Debug.Log(shortestDistanceIndex);
        DiceReset();
        return AttachPattern(shortestDistanceIndex);

    }

    // Commented pattern is the pattern that the faces are created in unity
    // Pattern is flipped to return the opposite side -- If the 1 on a d6 is on the ground (What i'm checking for) it'll return a 6 because that's what's facing up
    private int AttachPattern(int num)
    {
        
        switch (diceType)
        {
            case DiceType.D4:
                var d4 = new int[] { 4, 3, 2, 1 };
                return d4[num-1]; 
            case DiceType.D6:
                //var d6 = new int[] { 2, 3, 4, 5, 6, 1 };
                var d6 = new int[] { 5, 4, 3, 2, 1, 6 };
                return d6[num - 1];
            case DiceType.D8:
                //var d8 = new int[] { 3, 6, 8, 7, 5, 2, 1, 4 };
                var d8 = new int[] { 6, 3, 1, 2, 4, 7, 8, 5 };
                return d8[num - 1];
            case DiceType.D10:
                //var d10 = new int[] { 0, 8, 3, 5, 2, 6, 9, 1, 4, 7 };
                var d10 = new int[] { 9, 1, 6, 4, 7, 3, 0, 8, 5, 2 };
                return d10[num - 1];
            case DiceType.D12:
                //var d12 = new int[] { 1, 2, 4, 3, 8, 6, 5, 10, 7, 9, 11, 12 };
                var d12 = new int[] { 12, 11, 9, 10, 5, 7, 8, 3, 6, 4, 2, 1 };
                return d12[num - 1];
            case DiceType.D20:
                //var d20 = new int[] { 1, 7, 19, 3, 17, 15, 13, 9, 16, 10, 5, 11, 6, 8, 12, 18, 4, 14, 20, 2 };
                var d20 = new int[] { 20, 14, 2, 18, 4, 6, 8, 12, 5, 11, 16, 10, 15, 13, 9, 3, 17, 7, 1, 19 };
                return d20[num - 1];
            default:
                return -1;
        }

    }

    void RemoveSphere(GameObject obj)
    {
        Destroy(obj, 2f);
    }
}

