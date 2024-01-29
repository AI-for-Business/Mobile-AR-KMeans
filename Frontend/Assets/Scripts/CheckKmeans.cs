using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckKmeans : MonoBehaviour
{
    public Material Unclicked;

    GameObject kmeans;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        try
        {
            kmeans = GameObject.Find("kmeans");
        } catch (NullReferenceException)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, kmeans.transform.position);

        if (distance < 0.11)
        {
            transform.Find("kmeansTest").tag = "true";
            ResetCard();
        }
    }

    public void ResetCard()
    {
        Transform FeatureSelections = transform.Find("FeatureSelections");
        GameObject FeatureContainer = transform.Find("FeatureContainer").gameObject;

        Destroy(transform.Find("VisualizeData(Clone)").gameObject);

        FeatureContainer.SetActive(true);

        foreach (Transform Feature in FeatureContainer.transform)
        {
            Feature.GetComponent<Renderer>().material = Unclicked;
        }

        GameObject XComp = new List<GameObject>(GameObject.FindGameObjectsWithTag("XComp")).Find(g => g.transform.IsChildOf(FeatureSelections));
        GameObject YComp = new List<GameObject>(GameObject.FindGameObjectsWithTag("YComp")).Find(g => g.transform.IsChildOf(FeatureSelections));
        GameObject ZComp = new List<GameObject>(GameObject.FindGameObjectsWithTag("ZComp")).Find(g => g.transform.IsChildOf(FeatureSelections));

        XComp.name = "XComp";
        YComp.name = "YComp";
        ZComp.name = "ZComp";
    }
}
