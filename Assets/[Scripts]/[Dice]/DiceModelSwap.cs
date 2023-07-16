using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DiceModelSwap : MonoBehaviour
{
    public List<Mesh> diceModelList = new List<Mesh>();

    private GameManager gameManager;

    //TODO make a UI swap method in the GameManager script. call that method inside of here using the dieIndex as an argument 

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get Die Index based on numeric keyboard input
        var dieIndex = -1;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            dieIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            dieIndex = 1;
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            dieIndex = 2;
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            dieIndex = 3;
        } else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            dieIndex = 4;
        } else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            dieIndex = 5;
        } else if (Input.GetKeyDown(KeyCode.Alpha7)) {
            dieIndex = 6;
        }

        // If input detected, move transform up a bit, and swap mesh (and meshCollider) from list
        if (dieIndex != -1) {
            this.gameObject.transform.position = new Vector3(
                this.gameObject.transform.position.x,
                this.gameObject.transform.position.y + 0.3f,
                this.gameObject.transform.position.z);
            var dieMesh = diceModelList[dieIndex];
            this.GetComponent<MeshFilter>().mesh = dieMesh;
            this.GetComponent<MeshCollider>().sharedMesh = dieMesh;

            //update UI
            gameManager.UIDieSwap(dieIndex);
        }
    }
}
