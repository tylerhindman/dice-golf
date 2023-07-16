using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class DiceGoalDetector : MonoBehaviour
{
    public TurnStateMachine stateMachine;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.name == "GoalVolume") {
            var goal = collision.gameObject;
            this.stateMachine.setNextState(TurnStateMachine.State.Pocket);
        }
    }

    void PocketState() {
        Debug.Log("What the hell...time for a top up");
    }
}
