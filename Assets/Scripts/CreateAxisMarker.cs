using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class CreateAxisMarker : MonoBehaviour
{
    public GameObject AxisMarker;

    public void CreateMarker(string Axis, float Position, float Value)
    {
        var Marker = Instantiate(AxisMarker, new Vector3(0, 0, 0), Quaternion.identity);
        
        var MarkerNumber = Marker.GetComponentsInChildren<Transform>()[1].transform.Find("Value").GetComponent<TMP_Text>();
        MarkerNumber.text = Math.Round(Value, 4).ToString();

        var NumberOfDigits = MarkerNumber.text.Length;
        if (NumberOfDigits > 2)
        {
            MarkerNumber.fontSize = 40;
            if (NumberOfDigits > 3)
            {
                MarkerNumber.fontSize = 30;
            }
            if (MarkerNumber.text.Contains(','))
            {
                MarkerNumber.fontSize += 5;
            }
        }
        

        //Feature.transform.parent = ParentObject.GetComponentsInChildren<Transform>()[1];
        
        if (Axis == "X")
        {
            Marker.transform.parent = GameObject.Find("MarkerContainerX").transform;
            Marker.transform.localScale = new Vector3(0.05f, 0.01f, 0.01f);
            Marker.transform.localPosition = new Vector3(Position , 0, 0.03f);
            Marker.transform.localRotation = Quaternion.Euler(0, 90, 0);
        }
        else if (Axis == "Y")
        {
            Marker.transform.parent = GameObject.Find("MarkerContainerY").transform;
            Marker.transform.localScale = new Vector3(0.05f, 0.01f, 0.01f);
            Marker.transform.localPosition = new Vector3(0.02f, Position, 0.02f);
            Marker.transform.localRotation = Quaternion.Euler(0, 135, 0);

            MarkerNumber.transform.localPosition = new Vector3(1.85f, 0.4f, -1.85f);
            MarkerNumber.transform.localRotation = Quaternion.Euler(0, -135, 0);
        }
        else if (Axis == "Z")
        {
            Marker.transform.parent = GameObject.Find("MarkerContainerZ").transform;
            Marker.transform.localScale = new Vector3(0.05f, 0.01f, 0.01f);
            Marker.transform.localPosition = new Vector3(0.03f, 0, Position);
            Marker.transform.localRotation = Quaternion.Euler(0, 180, 0);

            MarkerNumber.transform.localPosition = new Vector3(0.738f, 5.4f, 0);
            MarkerNumber.transform.localRotation = Quaternion.Euler(0, -180, 0);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
