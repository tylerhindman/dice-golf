using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // player data
    private int Score;
    private int Rolls;

    // UI hookups
    [Header("UI Hookups")]
    [SerializeField] TMP_Text currentDie;
    [SerializeField] TMP_Text score;
    [SerializeField] TMP_Text rollCount;
    
    void Awake()
    {
        // initialize

        // Lock the cursor to the screen
        Cursor.lockState = CursorLockMode.Locked;

        // Clear the Player's Score
        Score = 0;

        // Clear the Player's Rolls (Move Count)
        Rolls = 0;
    }

    
    void Update()
    {
        // Press Escape to toggle the cursor lock mode

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }



    public void IncreaseScore(int amount)
    {
        // public method to be called by level pickups that increase score

        Score = Score + amount;
    }

    public void DecreaseScore(int amount)
    {
        // public method to be called by level pickups that decrease score

        Score = Score - amount;
    }

    public void IncreaseRollCount()
    {
        // public method to be called when a player rolls
        
        Rolls++;
    }
}
