using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DiceModelSwap : MonoBehaviour
{
    public List<Mesh> diceModelList = new List<Mesh>();
    public List<Material> diceMaterialList = new List<Material>();

    private GameManager gameManager;
    private DiceStateMachine stateMachine;
    private int dieIndex = 0;

    private RayDice rayDice;

    //TODO make a UI swap method in the GameManager script. call that method inside of here using the dieIndex as an argument 

    // Start is called before the first frame update
    void Start()
    {
        this.stateMachine = this.GetComponent<DiceStateMachine>();
        this.gameManager = FindObjectOfType<GameManager>();
        rayDice = GetComponent<RayDice>();

        // Determine base dieIndex
        for (var i = 0; i < this.diceModelList.Count; i++) {
            if (this.GetComponent<MeshFilter>().mesh.ToString().Contains((new Regex(@"^(D.*)\s")).Match(this.diceModelList[i].ToString()).Groups[0].ToString())) {
                this.dieIndex = i;
                break;
            }
        }
        gameManager.UIDieSwap(this.dieIndex);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void swapModel() {
        this.gameObject.transform.position = new Vector3(
            this.gameObject.transform.position.x,
            this.gameObject.transform.position.y + 0.3f,
            this.gameObject.transform.position.z);
        var dieMesh = diceModelList[this.dieIndex];
        var dieMaterial = diceMaterialList[this.dieIndex];
        this.GetComponent<MeshFilter>().mesh = dieMesh;
        this.GetComponent<MeshCollider>().sharedMesh = dieMesh;
        this.GetComponent<MeshRenderer>().material = dieMaterial;

        //update UI
        gameManager.UIDieSwap(this.dieIndex);
    }

    public void swapLeft() {
        if (this.stateMachine.state == DiceStateMachine.State.Slingshot) {
            this.dieIndex = this.dieIndex != 0 ? this.dieIndex - 1 : this.diceModelList.Count - 1;
            this.swapModel();
            rayDice.DiceSwitch();
        }
    }

    public void swapRight() {
        if (this.stateMachine.state == DiceStateMachine.State.Slingshot) {
            this.dieIndex = this.dieIndex != (this.diceModelList.Count - 1) ? this.dieIndex + 1 : 0;
            this.swapModel();
            rayDice.DiceSwitch();
        }
    }
}
