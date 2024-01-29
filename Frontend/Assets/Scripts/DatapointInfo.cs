using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DatapointInfo : MonoBehaviour
{
    private static readonly List<Color> colors = new List<Color> { Color.green, Color.blue, Color.red, Color.yellow, Color.cyan };
    Camera arCam;
    GameObject ServerManager;

    public void ShowInfo()
    {
        if (transform.tag == "datapoint")
        {
            transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        }
        else
        {
            transform.localScale = new Vector3(0.075f, 0.075f, 0.075f);

            transform.Find("info").localPosition = new Vector3(0f, (1.2f - transform.localPosition.y) * 17.5f, 0f);

            var dist = Vector3.Distance(transform.Find("info").position, arCam.transform.position);
            transform.Find("info").localScale = new Vector3(dist * 2f, dist * 2f, dist * 2f);

            transform.Find("info").gameObject.SetActive(true);

            //transform.Find("info").gameObject.AddComponent<StickToScreen>();
        }
        if (transform.gameObject.tag != "centroid")
        {
            //transform.Find("info").localPosition = new Vector3(1f, -0.5f, 0f);
            var Centroids = GameObject.FindGameObjectsWithTag("centroid").OrderByDescending(c=>int.Parse(c.transform.Find("Index").GetChild(0).name));

            int i = 0;

            Dictionary<int, float> kmeansDistances = new Dictionary<int, float>();

            foreach (GameObject centroid in Centroids)
            {
                Debug.Log(centroid.transform.Find("Index").GetChild(0).name);
                GameObject line = new GameObject();
                line.transform.parent = centroid.transform;
                line.name = "line";
                LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
                lineRenderer.useWorldSpace = false;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.material.color = colors[i];
                lineRenderer.widthMultiplier = 0.001f;
                lineRenderer.SetPosition(0, centroid.transform.position);
                lineRenderer.SetPosition(1, transform.position);
                Vector3 middle = (centroid.transform.position + transform.position) / 2;
                GameObject textContainer = new GameObject();
                textContainer.name = "distance";
                textContainer.transform.SetParent(centroid.transform, false);
                textContainer.transform.position = middle;
                var text = textContainer.AddComponent<TextMeshPro>();
                text.GetComponent<RectTransform>().sizeDelta = new Vector2(3.3f, 1.5f);
                text.alignment = TextAlignmentOptions.Center;
                //text.transform.position = middle;
                float distance = Vector3.Distance(centroid.transform.position, transform.position) * 5f * 25f / 3f;
                string textString = distance.ToString("n2");
                text.text = textString;
                text.fontSize = 14;
                text.color = colors[i];
                text.AddComponent<LookAtCam>();
                GameObject textBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
                textBackground.transform.SetParent(textContainer.transform, false);
                textBackground.transform.localScale = new Vector3(3.3f, 0.2f, 0.1f);
                textBackground.transform.localPosition = 1.1f * textBackground.transform.position - new Vector3(0, 0.2f, 0); //+ (textBackground.transform.position - arCam.transform.position) * 0.01f;
                Color c = colors[i];
                c.a = 0.5f;
                textBackground.GetComponent<Renderer>().material.color = c;
                textBackground.AddComponent<LookAtCam>();

                kmeansDistances[i] = distance;

                i++;
            }

            ServerManager.GetComponent<PythonServer>().CreateLog("KMeans Distances", kmeansDistances:kmeansDistances);
        }
    }

    public void HideInfo()
    {
        if (transform.tag == "datapoint")
        {
            transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        } else if (transform.tag == "toClassify" || transform.tag == "classSet")
        {
            transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        } else
        {
            transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        }
        transform.Find("info").gameObject.SetActive(false);
        var Centroids = GameObject.FindGameObjectsWithTag("centroid");
        foreach (GameObject centroid in Centroids)
        {
            try
            {
                Destroy(centroid.transform.Find("line").gameObject);
                Destroy(centroid.transform.Find("distance").gameObject);
            } catch { }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();
        ServerManager = GameObject.Find("ServerManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
