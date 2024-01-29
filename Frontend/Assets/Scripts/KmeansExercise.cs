using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using static UnityEngine.GraphicsBuffer;
using static ClickManager;

public class KmeansExercise : MonoBehaviour
{
    public GameObject visualize;
    public GameObject DataCubePrefab;

    GameObject ServerManager;
    GameObject ClickManager;

    bool updateCentroids;
    bool moveCentroids;

    //public float speed = (float)Math.Pow(5000, 5000);
    //[Range(0f, 1000f)]
    //public float speed;

    //public float speed = (float)Math.Pow(10, 2);
    public float speed = (float)1;

    private Transform centroid1;
    private Transform centroid2;
    private Transform centroid3;

    private Vector3 centroid1Target;
    private Vector3 centroid2Target;
    private Vector3 centroid3Target;

    // Start is called before the first frame update
    void Start()
    {
        ServerManager = GameObject.Find("ServerManager");
        ClickManager = GameObject.Find("ClickManager");
        updateCentroids = false;
        moveCentroids = false;

        transform.Find("Converged").gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckDatapoints())
        {
            if (!updateCentroids)
            {
                setUpdateCentroids();
            }
            updateCentroids = true;
        } else if (updateCentroids)
        {
            if ((!CheckDatapoints()))
            {   
                hideUpdateCentroids();
                updateCentroids = false;
            }
        }

        if (moveCentroids)
        {
            Debug.Log("Position: " + centroid1.localPosition.ToString());
            Debug.Log("Target: " + centroid1Target.ToString());
            var step = speed * Time.deltaTime; // calculate distance to move
            centroid1.localPosition = Vector3.MoveTowards(centroid1.localPosition, centroid1Target, step);
            centroid2.localPosition = Vector3.MoveTowards(centroid2.localPosition, centroid2Target, step);
            centroid3.localPosition = Vector3.MoveTowards(centroid3.localPosition, centroid3Target, step);
            if (Vector3.Distance(centroid1.localPosition, centroid1Target) < 0.0001f
                && Vector3.Distance(centroid2.localPosition, centroid2Target) < 0.0001f
                && Vector3.Distance(centroid3.localPosition, centroid3Target) < 0.0001f)
            {
                // Swap the position of the cylinder.
                moveCentroids = false;
            }
        }
    }

    public void StartExercise()
    {
        try
        {
            DestroyImmediate(transform.Find("visualize(Clone)").gameObject);
        }
        catch
        {
            Debug.Log("No object to be destroyed");
        }
        finally
        {
            var visualizeInst = Instantiate(visualize, transform.Find("Explanation").position, Quaternion.identity);
            transform.Find("Explanation").gameObject.SetActive(false);
            transform.Find("Converged").gameObject.SetActive(false);
            visualizeInst.transform.Find("kmeansTest").gameObject.tag = "exercise";
            //visualizeInst.transform.SetParent(transform, false);
            visualizeInst.transform.parent = transform;
            visualizeInst.transform.Find("FeatureContainer").gameObject.SetActive(false);
            //transform.localScale = new Vector3(10f, 10f, 10f);
            var DataCube = Instantiate(DataCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            DataCube.transform.parent = visualizeInst.transform;
            DataCube.transform.localPosition = new Vector3(0, -0.7f, 0);
            DataCube.transform.Find("Legend").gameObject.SetActive(false);
            visualizeInst.transform.Find("Dataset").name = "custom_kmeansExercise";
            ServerManager.GetComponent<PythonServer>().Visualize3DDataFromServerXYZ(visualizeInst, "a", "b", "c");
        }
    }

    public void setUpdateCentroids()
    {
        transform.Find("visualize(Clone)/VisualizeData(Clone)/UpdateCentroids").gameObject.SetActive(true);
        //ClickManager.GetComponent<ClickManager>().setAllDatapointsSet(true);
    }

    public void hideUpdateCentroids()
    {
        transform.Find("visualize(Clone)/VisualizeData(Clone)/UpdateCentroids").gameObject.SetActive(false);
        //ClickManager.GetComponent<ClickManager>().setAllDatapointsSet(false);
    }

    public bool CheckDatapoints()
    {
        var dataContainer = transform.Find("visualize(Clone)/VisualizeData(Clone)/DataPoints3D/Data");
        int numberToClassify = CountChildObjectsByTag(dataContainer.gameObject, "toClassify");
        if (numberToClassify > 0)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public void setCentroidMovement(List<GameObject> centroids, List<Vector3> centroidCoords)
    {
        centroid1 = centroids[0].transform;
        centroid2 = centroids[1].transform;
        centroid3 = centroids[2].transform;

        centroid1Target = centroidCoords[0];
        centroid2Target = centroidCoords[1];
        centroid3Target = centroidCoords[2];

        if (Vector3.Distance(centroid1.localPosition, centroid1Target) < 0.0001f && Vector3.Distance(centroid2.localPosition, centroid2Target) < 0.0001f && Vector3.Distance(centroid3.localPosition, centroid3Target) < 0.0001f)
        {
            transform.Find("Converged").gameObject.SetActive(true);
            transform.Find("visualize(Clone)/VisualizeData(Clone)/ResetDatapoints").gameObject.SetActive(false);
            return;
        }

        moveCentroids = true;
    }

    int CountChildObjectsByTag(GameObject parent, string tag)
    {
        int childCount = 0;
        foreach (Transform child in parent.transform)
        {
            if (child.CompareTag(tag))
                childCount++;
        }
        return childCount;
    }
}



/* TODOS:
 *  Test Vuforia
 *  Check if centroids aren't changing anymore and terminate algorithm
*/