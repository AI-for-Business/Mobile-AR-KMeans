using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR.Input;

public class SpawnableManager : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager m_RaycastManager;
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    [SerializeField]
    GameObject spawnablePrefab;
    [SerializeField]
    GameObject featureSelectionPrefab;

    Camera arCam;
    GameObject spawnedObject;
    GameObject featureSelection;

    GameObject ServerManager;

    // Start is called before the first frame update
    void Start()
    {
        spawnedObject = null;
        featureSelection = null;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
        ServerManager = GameObject.Find("ServerManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0)
            return;

        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        if (m_RaycastManager.Raycast(Input.GetTouch(0).position, m_Hits))
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began && spawnedObject == null)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Spawnable")
                    {
                        spawnedObject = hit.collider.gameObject;
                    }
                    else
                    {
                        SpawnPrefab(m_Hits[0].pose.position + ray.direction * 10.0f);
                    }
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && spawnedObject != null)
            {
                spawnedObject.transform.position = m_Hits[0].pose.position;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                spawnedObject = null;
            }
        }
    }

    private void SelectFeatures(Vector3 spawnPosition)
    {
        featureSelection = Instantiate(featureSelectionPrefab, spawnPosition, Quaternion.identity);
        ServerManager.GetComponent<PythonServer>().LoadDatasetMeta(featureSelection, "iris");
    }

    private void SpawnPrefab(Vector3 spawnPosition)
    {
        Debug.Log("Spawn Position: " + spawnPosition.ToString());
        spawnedObject = Instantiate(spawnablePrefab, spawnPosition, Quaternion.identity);
        ServerManager.GetComponent<PythonServer>().OnDataCubeInitialize();
    }
}
