using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class CheckDataset : MonoBehaviour
{
    public GameObject visualize;
    public GameObject DataCubePrefab;

    public Material Clicked;

    GameObject[] datasetCards;

    GameObject ServerManager;

    bool datasetFound;

    List<string> datasetsUsed;

    // Start is called before the first frame update
    void Start()
    {
        ServerManager = GameObject.Find("ServerManager");
        datasetFound = false;
        datasetsUsed = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        if (true)
        {
            try
            {
                datasetCards = GameObject.FindGameObjectsWithTag("datasetCard");
            } catch (NullReferenceException)
            {
                return;
            }

            foreach (GameObject card in datasetCards)
            {
                if (!datasetsUsed.Contains(card.name))
                {
                    float distance = Vector3.Distance(transform.position, card.transform.position);

                    if (distance < 0.12)
                    {
                        ConvertKmeans(card);
                    }
                }
            }
        }
    }

    public async void ConvertKmeans(GameObject card)
    {
        datasetFound = true;
        datasetsUsed.Add(card.name);
        transform.Find("Explanation").gameObject.SetActive(false);
        var visualizeInst = Instantiate(visualize);
        //Debug.Log("##### Position: " + visualizeInst.transform.position.ToString());
        //Debug.Log("##### Position: " + transform.Find("Explanation").position.ToString());
        //visualizeInst.transform.localPosition = transform.Find("Explanation").position;
        //visualizeInst.transform.parent = transform;
        visualizeInst.transform.SetParent(transform, false);
        //visualizeInst.transform.parent = GameObject.Find("GameObjectContainer").transform;
        await ServerManager.GetComponent<PythonServer>().LoadDatasetMeta(visualizeInst, card.name.Split("_")[1]);
        var FeatureSelections = card.transform.Find("FeatureSelections");
        var XComp = visualizeInst.transform.Find("FeatureSelections/XComp");
        var YComp = visualizeInst.transform.Find("FeatureSelections/YComp");
        var ZComp = visualizeInst.transform.Find("FeatureSelections/ZComp");
        XComp.name = new List<GameObject>(GameObject.FindGameObjectsWithTag("XComp")).Find(g => g.transform.IsChildOf(FeatureSelections)).name;
        YComp.name = new List<GameObject>(GameObject.FindGameObjectsWithTag("YComp")).Find(g => g.transform.IsChildOf(FeatureSelections)).name;
        ZComp.name = new List<GameObject>(GameObject.FindGameObjectsWithTag("ZComp")).Find(g => g.transform.IsChildOf(FeatureSelections)).name;
        visualizeInst.transform.Find("kmeansTest").tag = "true";

        if (XComp.name != "XComp" && YComp.name != "YComp" && ZComp.name != "ZComp")
        {
            visualizeInst.transform.Find("FeatureContainer").gameObject.SetActive(false);
            var DataCube = Instantiate(DataCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            DataCube.transform.parent = visualizeInst.transform;
            DataCube.transform.localPosition = new Vector3(0, -0.7f, 0);
            ServerManager.GetComponent<PythonServer>().Visualize3DDataFromServerXYZ(visualizeInst, XComp.name, YComp.name, ZComp.name);
        }
        else
        {
            foreach (GameObject feature in card.transform.Find("FeatureSelections"))
            {
                if (feature.name == XComp.name || feature.name == YComp.name || feature.name == ZComp.name)
                    feature.GetComponent<Renderer>().material = Clicked;
            }
        }
    }
}
