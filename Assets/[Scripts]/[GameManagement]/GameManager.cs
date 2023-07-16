using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // player data
    private int Score;
    private int Rolls;
    private TurnStateMachine stateMachine;

    // UI hookups
    [Header("UI Hookups")]
    [SerializeField] TMP_Text UICurrentDie;
    [SerializeField] TMP_Text UIScore;
    [SerializeField] TMP_Text UIRollCount;
    
    void Awake()
    {
        // initialize

        // Lock the cursor to the screen
        Cursor.lockState = CursorLockMode.Locked;

        // Clear the Player's Score
        Score = 0;

        // Clear the Player's Rolls (Move Count)
        Rolls = 0;

        //Set Default UI
        UICurrentDie.text = "Equipped: d6";
    }

    void Start() {
        this.stateMachine = FindObjectOfType<TurnStateMachine>();
    }

    
    void Update()
    {
        // Press Escape to toggle the cursor lock mode
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }

        // Update the UI every frame
        UpdateUI();
    }

    public void UpdateUI()
    {
        // Score UI equals score value
        UIScore.text = Score.ToString();

        // Roll UI equals roll count
        UIRollCount.text = Rolls.ToString();
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

    public void UIDieSwap(int dieIndex)
    {
        if (dieIndex == -1) UICurrentDie.text = "Equipped: d6";
        if (dieIndex == 0) UICurrentDie.text = "Equipped: d4";
        if (dieIndex == 1) UICurrentDie.text = "Equipped: d6";
        if (dieIndex == 2) UICurrentDie.text = "Equipped: d6b";
        if (dieIndex == 3) UICurrentDie.text = "Equipped: d8";
        if (dieIndex == 4) UICurrentDie.text = "Equipped: d10";
        if (dieIndex == 5) UICurrentDie.text = "Equipped: d12";
        if (dieIndex == 6) UICurrentDie.text = "Equipped: d20";
    }
}
