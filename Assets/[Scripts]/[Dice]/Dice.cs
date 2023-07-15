using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Dice : MonoBehaviour
{
    public event Action<int,Color> DiceValueReturned = delegate { };

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

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mr = GetComponent<MeshRenderer>();
        initPosition = transform.position;
        _rb.useGravity = false;
    }

    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.R))
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
            GetDiceValue();
        }
        // If the dice stopped moving and is stuck, reroll the dice
        else if (_rb.IsSleeping() && _hasLanded && _diceValue == -1)
        {
            RollAgain();
        }
    }

    public void RollDice()
    {
        // Set rigidbody bools to allow movement, add force + torque
        // Will not roll if the dice is in the process of being thrown or already landed
        if(!_thrown && !_hasLanded)
        {
            _thrown = true;
            _rb.isKinematic = false;
            _rb.useGravity = true;
            _rb.AddTorque(UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(0, 50));
            _rb.AddForce(UnityEngine.Random.Range(-25, 25), 0, UnityEngine.Random.Range(10, 25));

            //AudioManager.Instance.PlaySound2D(_diceThrowSFX, .5f, UnityEngine.Random.Range(.8f, 1.2f));
        }
        else if(_thrown && _hasLanded)
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
    public int GetDiceValue()
    {
        int iValue = -1;
        Vector3 dieRotation = gameObject.transform.eulerAngles;

        dieRotation = new Vector3(Mathf.RoundToInt(dieRotation.x), Mathf.RoundToInt(dieRotation.y), Mathf.RoundToInt(dieRotation.z));

        if (dieRotation.x == 180 && dieRotation.y == 270 ||
            dieRotation.x == 0 && dieRotation.z == 90)
        {
            iValue = 1;
        }
        else if (dieRotation.x == 270)
        {
            iValue = 2;
        }
        else if (dieRotation.x == 180 && dieRotation.z == 0 ||
          dieRotation.x == 0 && dieRotation.z == 180)
        {
            iValue = 3;
        }
        else if (dieRotation.x == 180 && dieRotation.z == 180 ||
          dieRotation.x == 0 && dieRotation.z == 0)
        {
            iValue = 4;
        }
        else if (dieRotation.x == 90)
        {
            iValue = 5;
        }
        else if (dieRotation.x == 0 && dieRotation.z == 270 ||
          dieRotation.x == 180 && dieRotation.z == 90)
        {
            iValue = 6;
        }

        //Debug.Log("eulerAngles: " + dieRotation.x + ", " + dieRotation.y + ", " + dieRotation.z + " Value: " + iValue);

        _diceValue = iValue;

        DiceValueReturned?.Invoke(_diceValue, _mr.material.color);

        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);

        ParticleSystem particles = Instantiate(_resolveParticles, transform.position, Quaternion.identity);

        ParticleSystem.MainModule settings = particles.main;
        settings.startColor = new ParticleSystem.MinMaxGradient(_mr.material.color);

        ScorePopup popup = Instantiate(_resolvePopup, pos, Quaternion.identity).GetComponent<ScorePopup>();

        popup.transform.rotation = Quaternion.Euler(45, 0, 0);

        popup.text.text = ("[<color=#" + ColorUtility.ToHtmlStringRGB(_mr.material.color) + ">" + iValue + "</color>]");
        popup.text.color = Color.white;
        popup.text.outlineColor = Color.black;

        //AudioManager.Instance.PlaySound2D(_diceResolveSFX, .5f, UnityEngine.Random.Range(.8f,1.2f));

        return (iValue);
    }

    // If dice is out of bounds, reroll
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "FallZone")
        {
            RollAgain();
        }
    }
}
