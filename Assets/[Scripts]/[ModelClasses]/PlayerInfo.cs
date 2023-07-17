using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public string playerButtonAxis;
    public string playerStickAxis;

    private int playerNumber;
    public int PlayerNumber {
        get { return this.playerNumber;}
        set {
            this.playerNumber = value;
            switch (this.playerNumber) {
                case 0:
                    this.playerButtonAxis = "Player 1 Button";
                    this.playerStickAxis = "Player 1 Stick";
                    break;
                case 1:
                    this.playerButtonAxis = "Player 2 Button";
                    this.playerStickAxis = "Player 2 Stick";
                    break;
                case 2:
                    this.playerButtonAxis = "Player 3 Button";
                    this.playerStickAxis = "Player 3 Stick";
                    break;
                case 3:
                    this.playerButtonAxis = "Player 4 Button";
                    this.playerStickAxis = "Player 4 Stick";
                    break;
            }
        }
    }
}
