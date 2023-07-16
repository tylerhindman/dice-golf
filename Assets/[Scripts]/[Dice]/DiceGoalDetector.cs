using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class DiceGoalDetector : MonoBehaviour
{
    private TurnStateMachine stateMachine;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        this.stateMachine = FindObjectOfType<TurnStateMachine>();

        this.gameManager = FindObjectOfType<GameManager>();
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
}
