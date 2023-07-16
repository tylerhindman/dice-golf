using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // player data
    private int Score;
    private int Rolls;
    private TurnStateMachine stateMachine;

    // UI hookups
    [Header("UI Hookups")]
    [SerializeField] Image UICurrentDieImage;
    [SerializeField] TMP_Text UICurrentDie;
    [SerializeField] TMP_Text UIScore;
    [SerializeField] TMP_Text UIRollCount;

    // UI Die Sprites
    [Header("UI Die Sprites")]
    [SerializeField] Sprite spriteD4;
    [SerializeField] Sprite spriteD6;
    [SerializeField] Sprite spriteD6b;
    [SerializeField] Sprite spriteD8;
    [SerializeField] Sprite spriteD10;
    [SerializeField] Sprite spriteD12;
    [SerializeField] Sprite spriteD20;
    
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
        UICurrentDieImage.sprite = spriteD6;
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
        //D6 is the default starting position

        if (dieIndex == -1)
        {
            UICurrentDie.text = "Equipped: d6";
            UICurrentDieImage.sprite = spriteD6;
        }

        //D4

        if (dieIndex == 0)
        {
            UICurrentDie.text = "Equipped: d4";
            UICurrentDieImage.sprite = spriteD4;
        }

        //D6

        if (dieIndex == 1)
        {
            UICurrentDie.text = "Equipped: d6";
            UICurrentDieImage.sprite = spriteD6;
        }

        //D6  (beveled)

        if (dieIndex == 2)
        {
            UICurrentDie.text = "Equipped: d6b";
            UICurrentDieImage.sprite = spriteD6b;
        }

        //D8

        if (dieIndex == 3)
        {
            UICurrentDie.text = "Equipped: d8";
            UICurrentDieImage.sprite = spriteD8;
        }

        //D10

        if (dieIndex == 4)
        {
            UICurrentDie.text = "Equipped: d10";
            UICurrentDieImage.sprite = spriteD10;
        }

        //D12

        if (dieIndex == 5)
        {
            UICurrentDie.text = "Equipped: d12";
            UICurrentDieImage.sprite = spriteD12;
        }

        //D20

        if (dieIndex == 6)
        {
            UICurrentDie.text = "Equipped: d20";
            UICurrentDieImage.sprite = spriteD20;
        }

    }
}
