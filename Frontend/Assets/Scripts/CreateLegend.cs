using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLegend : MonoBehaviour
{
    public GameObject LegendEntry;

    public void CreateLegendEntry(int Position, string name, List<Color> colors)
    {
        var LegendObj = Instantiate(LegendEntry, new Vector3(0, 0, 0), Quaternion.identity);

        LegendObj.transform.parent = GameObject.Find("LegendContainer").transform;
        LegendObj.transform.localPosition = new Vector3(0, Position * -2.25f, 0);
        LegendObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
        LegendObj.transform.localScale = new Vector3(1, 1, 1);

        var DatasetName = LegendObj.transform.Find("TargetName").GetComponent<TextMesh>();
        DatasetName.text = name;

        LegendObj.transform.Find("Sphere").GetComponent<Renderer>().material.SetColor("_Color", colors[Position]);
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
