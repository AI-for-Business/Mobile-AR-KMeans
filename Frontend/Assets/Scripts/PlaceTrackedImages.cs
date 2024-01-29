using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class PlaceTrackedImages : MonoBehaviour
{
    // Reference to AR tracked image manager component
    private ARTrackedImageManager _trackedImageManager;

    public GameObject[] ArPrefabs;

    private readonly Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>();

    GameObject ServerManager;

    private void Awake()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        _trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            var imageName = trackedImage.referenceImage.name;

            foreach (var curPrefab in ArPrefabs)
            {
                if (string.Compare(curPrefab.name, imageName.Split("_")[0], System.StringComparison.OrdinalIgnoreCase) == 0
                    && !_instantiatedPrefabs.ContainsKey(imageName))
                {
                    var newPrefab = Instantiate(curPrefab, trackedImage.transform);
                    newPrefab.transform.name = imageName;
                    _instantiatedPrefabs[imageName] = newPrefab;

                    /*if (imageName.Split("_").Count() > 1 && 
                        imageName.Split("_")[1].Split("!").Count() > 1)
                    {
                        GameObject.Find(imageName.Split("!")[0]).SetActive(false);
                    }*/


                    LoadMeta(newPrefab, imageName);

                    //ServerManager.GetComponent<PythonServer>().OnDataCubeInitialize();
                }
            }
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            _instantiatedPrefabs[trackedImage.referenceImage.name]
                .SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            Destroy(_instantiatedPrefabs[trackedImage.referenceImage.name]);
            _instantiatedPrefabs.Remove(trackedImage.referenceImage.name);
        }
    }

    public async void LoadMeta(GameObject newPrefab, string imageName)
    {
        await ServerManager.GetComponent<PythonServer>().LoadDatasetMeta(newPrefab, imageName.Split("_")[1]);
    }

    // Start is called before the first frame update
    void Start()
    {
        ServerManager = GameObject.Find("ServerManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
