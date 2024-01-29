using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class CreateFeatures : MonoBehaviour
{
    public GameObject FeatureCube;

    //GameObject ServerManager = GameObject.Find("ServerManager");

    public void CreateVariableBox(string VariableName, GameObject ParentObject, int Position, double Columns, int Column)
    {
        float ColumnPos = (float)(Column - (Columns + 1) / 2.0);

        var Feature = Instantiate(FeatureCube, new Vector3(0, 0, 0), Quaternion.identity);

        //Debug.Log(Position);

        Feature.transform.parent = ParentObject.transform.Find("FeatureContainer");
        Feature.transform.localPosition = new Vector3(ColumnPos * 6.0f, 2.0f + Position * -0.5f, 0);
        //Feature.transform.localScale = new Vector3(0.15f, 0.1f, 0.02f);
        Feature.transform.localScale = Feature.transform.localScale * 0.025f;
        Feature.transform.localRotation = Quaternion.Euler(0, 0, 0);

        var FeatureName = Feature.transform.Find("FeatureName").GetComponent<TMP_Text>();
        FeatureName.text = VariableName;
        Feature.name = VariableName;
        var Pos = Feature.transform.Find("Pos");
        Pos.localPosition = Feature.transform.localPosition;

        Feature.tag = "Feature";
    }

    private void Awake()
    {
        //ServerManager.GetComponent<PythonServer>().LoadDatasetMeta(this.transform, "iris");
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
