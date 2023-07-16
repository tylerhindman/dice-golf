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

    // Will be overwritten with Mikey's script later
    void ResolveState()
    {
        this.state = State.Slingshot;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.state = State.Slingshot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setNextState(State nextState)
    {
        this.state = nextState;
        SendMessage(this.state.ToString() + "State", SendMessageOptions.DontRequireReceiver);
    }
}
