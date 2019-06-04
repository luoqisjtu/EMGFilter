using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRacket : MonoBehaviour
{

    // Use this for initialization

    public GameObject obj;
    public Renderer rend;
    void Start()
    {

        obj = GameObject.Find("TargetRacket");
        rend = obj.GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        rend.material.color = Color.red;

    }
    void OnCollisionExit2D(Collision2D col)
    {
        rend.material.color = Color.green;

    }

}