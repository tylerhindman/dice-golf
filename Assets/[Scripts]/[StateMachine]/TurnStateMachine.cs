using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnStateMachine : MonoBehaviour
{
    private List<GameObject> interestedParties = new List<GameObject>();

    public enum State
    {
        DiceSwap,
        Slingshot,
        Rolling,
        Resolve,
        Pocket,
        PlayerSwitch,
        LevelEnd
    }

    public State state;

    IEnumerator DiceSwapState()
    {
        Debug.Log("DiceSwap: Enter");
        while (state == State.DiceSwap)
        {
            yield return 0;
        }
        Debug.Log("DiceSwap: Exit");
        //NextState();
    }

    IEnumerator SlingshotState()
    {
        Debug.Log("Slingshot: Enter");
        while (state == State.Slingshot)
        {
            yield return 0;
        }
        Debug.Log("Slingshot: Exit");
        //NextState();
    }

    IEnumerator RollingState()
    {
        Debug.Log("Rolling: Enter");
        while (state == State.Rolling)
        {
            yield return 0;
        }
        Debug.Log("Rolling: Exit");
    }

    IEnumerator ResolveState()
    {
        Debug.Log("Resolve: Enter");
        while (state == State.Resolve)
        {
            yield return 0;
        }
        Debug.Log("Resolve: Exit");
    }

    IEnumerator PocketState()
    {
        Debug.Log("Pocket: Enter");
        while (state == State.Pocket)
        {
            yield return 0;
        }
        Debug.Log("Pocket: Exit");
    }

    IEnumerator PlayerSwitchState()
    {
        Debug.Log("PlayerSwitch: Enter");
        while (state == State.PlayerSwitch)
        {
            yield return 0;
        }
        Debug.Log("PlayerSwitch: Exit");
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
