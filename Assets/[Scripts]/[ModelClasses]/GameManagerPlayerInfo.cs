using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerPlayerInfo : MonoBehaviour
{
    public bool levelFinished = false;
    public GameObject player;

    public GameManagerPlayerInfo(GameObject player) {
        this.player = player;
    }
}
