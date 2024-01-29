using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
//using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR.Input;
using UnityEngineInternal;

public class ClickManager : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    public GameObject DataCubePrefab;

    public Material Clicked;
    public Material Unclicked;

    Camera arCam;
    GameObject ServerManager;
    bool next;
    bool keepInfo;

    bool allDatapointsSet;

    private List<GameObject> InfoBuffer = new List<GameObject>();

    private static readonly List<Color> colors = new List<Color> { Color.green, Color.blue, Color.red, Color.yellow, Color.cyan };

    // Start is called before the first frame update
    void Start()
    {
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
        //arCam = GetComponent<ARSessionOrigin>().camera;
        ServerManager = GameObject.Find("ServerManager");
        next = false;
        keepInfo = false;
        allDatapointsSet = false;
    }

    // Update is called once per frame
    async void Update()
    {
        if (Input.touchCount == 0)
            return;

        RaycastHit hit;
        var screenPosition = Input.GetTouch(0).position;
        Ray ray = arCam.ScreenPointToRay(screenPosition);

        //if (m_RaycastManager.Raycast(ray, m_Hits, TrackableType.AllTypes))
        if (true)
        {
            //Debug.Log("##### Number of hits:" + m_Hits.Count.ToString());
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                /*if (InfoBuffer.Count > 0 && !keepInfo)
                {
                    foreach (GameObject go in InfoBuffer)
                    {
                        go.GetComponent<DatapointInfo>().HideInfo();
                    }
                    InfoBuffer.Clear();
                }*/
                
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Name: " + hit.collider.gameObject.name);
                    Debug.Log("Tag: " + hit.collider.gameObject.tag);
                    ServerManager.GetComponent<PythonServer>().CreateLog("Click", hit.collider.gameObject.name + "#divider#" + hit.collider.gameObject.tag);
                    
                    if (hit.collider.gameObject.tag != "setClass")
                    {
                        foreach (GameObject go in GameObject.FindGameObjectsWithTag("toClassify"))
                        {
                            go.GetComponent<DatapointInfo>().HideInfo();
                        }
                        foreach (GameObject go in GameObject.FindGameObjectsWithTag("classSet"))
                        {
                            go.GetComponent<DatapointInfo>().HideInfo();
                        }
                        foreach (GameObject go in GameObject.FindGameObjectsWithTag("datapoint"))
                        {
                            go.GetComponent<DatapointInfo>().HideInfo();
                        }
                        foreach (GameObject go in GameObject.FindGameObjectsWithTag("centroid"))
                        {
                            go.GetComponent<DatapointInfo>().HideInfo();
                        }
                    }
                    // TODO: global variable for tracking of data point info and class selection

                    if (hit.collider.gameObject.tag == "Feature")
                    {
                        SetFeatures(hit.collider.gameObject);
                    }
                    else if (hit.collider.gameObject.tag == "datapoint" || hit.collider.gameObject.tag == "centroid")
                    {
                        Debug.Log("----- datapoint / centroid clicked");
                        InfoBuffer.Add(hit.collider.gameObject);
                        hit.collider.gameObject.GetComponent<DatapointInfo>().ShowInfo();
                    }
                    else if (hit.collider.gameObject.tag == "toClassify" || hit.collider.gameObject.tag == "classSet")
                    {
                        Debug.Log("----- toClassify clicked");
                        InfoBuffer.Add(hit.collider.gameObject);
                        if (!allDatapointsSet)
                        {
                            hit.collider.gameObject.GetComponent<DatapointInfo>().ShowInfo();
                            keepInfo = true;
                        }
                        
                    }
                    else if (hit.collider.gameObject.name == "Next")
                    {
                        Debug.Log("----- next clicked");
                        next = true;
                        await Task.Delay(TimeSpan.FromSeconds(0.15));
                        next = false;
                    }
                    else if (hit.collider.gameObject.transform.parent.parent.name == "kmeansExercise")
                    {
                        Debug.Log("----- kmeansExercise clicked");
                        if (hit.collider.gameObject.transform.parent.name == "Explanation_1" || hit.collider.gameObject.transform.parent.name == "Converged")
                        {
                            hit.collider.gameObject.transform.parent.gameObject.SetActive(false);
                            hit.collider.gameObject.transform.parent.parent.GetComponent<KmeansExercise>().StartExercise();
                        } else if (hit.collider.gameObject.transform.parent.name == "Explanation")
                        {
                            hit.collider.gameObject.transform.parent.gameObject.SetActive(false);
                            hit.collider.gameObject.transform.parent.parent.Find("Explanation_1").gameObject.SetActive(true);
                        }
                    }
                    else if (hit.collider.gameObject.tag == "setClass")
                    {
                        int classInt = int.Parse(hit.collider.gameObject.name.Replace("Class", ""));
                        var datapoint = hit.collider.gameObject.transform.parent.parent;
                        var DataRenderer = datapoint.GetComponent<Renderer>();
                        DataRenderer.material.SetColor("_Color", colors[classInt]);
                        datapoint.Find("Target").GetChild(0).name = classInt.ToString();
                        datapoint.tag = "classSet";
                        keepInfo = false;
                        hit.collider.gameObject.transform.parent.parent.GetComponent<DatapointInfo>().HideInfo();
                    }
                    else if (hit.collider.gameObject.transform.parent.name == "UpdateCentroids")
                    {
                        var visualize = hit.collider.gameObject.transform.parent.parent;
                        visualize.Find("UpdateCentroids").gameObject.SetActive(false);
                        ServerManager.GetComponent<PythonServer>().UpdateCentroids(visualize.gameObject);
                        visualize.Find("ResetDatapoints").gameObject.SetActive(true);
                        setAllDatapointsSet(true);
                    }
                    else if (hit.collider.gameObject.transform.parent.name == "ResetDatapoints")
                    {
                        var visualize = hit.collider.gameObject.transform.parent.parent;
                        ServerManager.GetComponent<PythonServer>().ResetDatapoints(visualize.gameObject);
                        visualize.Find("ResetDatapoints").gameObject.SetActive(false);
                        setAllDatapointsSet(false);
                    }
                    else
                    {
                        Debug.Log(hit.collider.gameObject.name);
                        //hit.collider.gameObject.transform.GetComponent<Renderer>().material.color = Color.red;
                    }
                }
                else
                {
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("toClassify"))
                    {
                        go.GetComponent<DatapointInfo>().HideInfo();
                    }
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("classSet"))
                    {
                        go.GetComponent<DatapointInfo>().HideInfo();
                    }
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("datapoint"))
                    {
                        go.GetComponent<DatapointInfo>().HideInfo();
                    }
                    foreach (GameObject go in GameObject.FindGameObjectsWithTag("centroid"))
                    {
                        go.GetComponent<DatapointInfo>().HideInfo();
                    }
                }
            }
        }
    }

    public bool getNext()
    {
        return next;
    }

    public void setAllDatapointsSet(bool allSet)
    {
        allDatapointsSet = allSet;
    }

    public void SetFeatures(GameObject Feature)
    {
        GameObject ParentContainer = Feature.transform.parent.parent.gameObject;
        GameObject FeatureContainer = Feature.transform.parent.gameObject;
        Transform FeatureSelections = ParentContainer.transform.Find("FeatureSelections");
        GameObject XComp = new List<GameObject>(GameObject.FindGameObjectsWithTag("XComp")).Find(g => g.transform.IsChildOf(FeatureSelections));
        GameObject YComp = new List<GameObject>(GameObject.FindGameObjectsWithTag("YComp")).Find(g => g.transform.IsChildOf(FeatureSelections));
        GameObject ZComp = new List<GameObject>(GameObject.FindGameObjectsWithTag("ZComp")).Find(g => g.transform.IsChildOf(FeatureSelections));

        //Material Clicked = Resources.Load("DataSelectorClicked", typeof(Material)) as Material;
        //Material Unclicked = Resources.Load("DataSelectorUnclicked", typeof(Material)) as Material;

        if (Feature.GetComponent<Renderer>().sharedMaterial == Unclicked)
        {
            Feature.GetComponent<Renderer>().material = Clicked;
            if (XComp.name == "XComp")
            {
                XComp.name = Feature.name;
            } else if(YComp.name == "YComp")
            {
                YComp.name = Feature.name;
            } else if (ZComp.name == "ZComp")
            {
                ZComp.name = Feature.name;
            }
        } else if (Feature.GetComponent<Renderer>().sharedMaterial == Clicked)
        {
            Feature.GetComponent<Renderer>().material = Unclicked;
            foreach (GameObject Comp in FeatureSelections)
            {
                if (Comp.name == Feature.name)
                {
                    Comp.name = Comp.tag;
                }
            }
        }
        else
        {
            Debug.Log(Feature.GetComponent<Renderer>().material);
        }

        if (XComp.name != "XComp" && YComp.name != "YComp" && ZComp.name != "ZComp")
        {
            FeatureContainer.SetActive(false);
            var DataCube = Instantiate(DataCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            DataCube.transform.parent = ParentContainer.transform;
            DataCube.transform.localPosition = new Vector3(0, -0.7f, 0);
            ServerManager.GetComponent<PythonServer>().Visualize3DDataFromServerXYZ(ParentContainer, XComp.name, YComp.name, ZComp.name);
        }
    }

}
