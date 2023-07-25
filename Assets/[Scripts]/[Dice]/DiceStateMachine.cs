using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceStateMachine : MonoBehaviour
{
    public enum State
    {
        Slingshot,
        Rolling,
        Resolve,
        Pocket
    }

    public State state;

    private GameManager gameManager;
    private RayDice diceResolver;

    // Start is called before the first frame update
    void Start()
    {
        this.state = State.Slingshot;
        this.gameManager = FindObjectOfType<GameManager>();
        this.diceResolver = GetComponent<RayDice>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ResolveState()
    {
        // update roll count in Game Manager
        this.gameManager.IncreaseRollCount();

        this.diceResolver.RollDice();
        
        this.setNextState(State.Slingshot);
    }

    void RollingState() {

    }

    public void setNextState(State nextState)
    {
        this.state = nextState;
        SendMessage(this.state.ToString() + "State", SendMessageOptions.DontRequireReceiver);
    }
}
