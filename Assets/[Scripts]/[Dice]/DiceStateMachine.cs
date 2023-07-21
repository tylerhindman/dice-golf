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

    // Start is called before the first frame update
    void Start()
    {
        this.state = State.Slingshot;
        this.gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Will be overwritten with Mikey's script later
    void ResolveState()
    {
        // update roll count in Game Manager
        this.gameManager.IncreaseRollCount();
        
        this.state = State.Slingshot;
    }

    public void setNextState(State nextState)
    {
        this.state = nextState;
        SendMessage(this.state.ToString() + "State", SendMessageOptions.DontRequireReceiver);
    }
}
