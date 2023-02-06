using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreateDatasetCubes : MonoBehaviour
{
    public GameObject DatasetCube;

    public void CreateDatasetBox(string DataName, int Position)
    {
        //float ColumnPos = (float)(Column - (Columns + 1) / 2.0);

        var Dataset = Instantiate(DatasetCube, new Vector3(0, 0, 0), Quaternion.identity);

        Dataset.tag = "DatasetCube";

        //Debug.Log(Position);

        Dataset.transform.parent = GameObject.Find("DatasetsContainer").transform;
        Dataset.transform.localPosition = new Vector3(0, -0.75f + (Position - 1) * -2f, 0);
        //Feature.transform.localScale = new Vector3(1.5f, 1f, 0.2f);
        Dataset.transform.localRotation = Quaternion.Euler(0, 0, 0);

        var DatasetName = Dataset.transform.Find("DatasetName").GetComponent<TMP_Text>();
        DatasetName.text = DataName;
        Dataset.name = "custom_" + DataName;
        var Pos = Dataset.transform.Find("Pos");
        Pos.localPosition = Dataset.transform.localPosition;
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
