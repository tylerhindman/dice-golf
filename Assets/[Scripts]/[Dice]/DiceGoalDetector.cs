using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class DiceGoalDetector : MonoBehaviour
{
    private DiceStateMachine stateMachine;
    private GameManager gameManager;
    private PlayerInfo playerInfo;
    // Start is called before the first frame update
    void Start()
    {
        this.stateMachine = this.GetComponent<DiceStateMachine>();
        this.playerInfo = this.GetComponent<PlayerInfo>();

        this.gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.name == "GoalVolume") {
            var goal = collision.gameObject;
            this.stateMachine.setNextState(DiceStateMachine.State.Pocket);
            this.gameManager.playerFinishedLevel(this.playerInfo.playerNumber);
        }
    }
}
