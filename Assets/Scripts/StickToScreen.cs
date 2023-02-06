using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickToScreen : MonoBehaviour
{
    Camera arCam;
    
    // Start is called before the first frame update
    void Start()
    {
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 1f));
        //transform.position = Vector3.MoveTowards(transform.position, arCam.transform.position + new Vector3(0, -0.05f, 0.05f), 0.01f);
    }
}
