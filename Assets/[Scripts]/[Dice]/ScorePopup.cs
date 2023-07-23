using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{

    public TMP_Text text;

    private void Awake()
    {
        Destroy(gameObject, .5f);
        text = GetComponent<TMP_Text>();
        text.outlineWidth = 0.15f;
    }

    private void Start()
    {
        //text.SetText("+" + text.text);
    }

    void Update()
    {
        transform.localPosition = new Vector3(transform.position.x, transform.position.y + 0.001f, transform.position.z);
        transform.localScale -= new Vector3(0.0005f, 0.0005f, 0.0005f);
        //text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - 0.005f);
    }
}
