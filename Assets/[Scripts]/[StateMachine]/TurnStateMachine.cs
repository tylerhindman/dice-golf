using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnStateMachine : MonoBehaviour
{
    private List<GameObject> interestedParties = new List<GameObject>();

    public enum State
    {
        Start,
        Chaos,
        LevelEnd
    }

    public State state;

    IEnumerator StartState()
    {
        Debug.Log("Start: Enter");
        while (state == State.Start)
        {
            yield return 0;
        }
        Debug.Log("Start: Exit");
    }

    IEnumerator ChaosState()
    {
        Debug.Log("Chaos: Enter");
        while (state == State.Chaos)
        {
            yield return 0;
        }
        Debug.Log("Chaos: Exit");
    }

    IEnumerator LevelEndState()
    {
        Debug.Log("LevelEnd: Enter");
        while (state == State.LevelEnd)
        {
            yield return 0;
        }
        Debug.Log("LevelEnd: Exit");
    }

    void Start()
    {
        communicateState();
    }

    void communicateState() {
        foreach (GameObject obj in interestedParties) {
            obj.SendMessage(this.state.ToString() + "State", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void setNextState(State nextState)
    {
        this.state = nextState;
        SendMessage(this.state.ToString() + "State");
        this.communicateState();
    }

    public void registerInterestedParty(GameObject obj) {
        this.interestedParties.Add(obj);
    }

}
