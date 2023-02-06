using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using TMPro;
using Newtonsoft.Json;
using System.Text;
using System.Threading;

using static PythonServerRequest;
using static LogMessage;
using static CreateFeatures;
using static CreateLegend;

using System.Data;
using System.Linq;
using UnityEngine.UIElements;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.Analytics;

public class PythonServer : MonoBehaviour
{
    [SerializeField]
    string m_DataCubeName;

    public GameObject DatapointInfo;
    public GameObject KmeansExInfo;

    private static readonly HttpClient client = new HttpClient();

    //KIT PC
    private static readonly string HostIP = "172.22.77.25";

    //KIT:
    //private static readonly string HostIP = "172.22.77.106";

    //KIT WLAN (changes)
    //private static readonly string HostIP = "172.17.115.87";

    //KA
    //private static readonly string HostIP = "192.168.178.35";


    //LA
    //private static readonly string HostIP = "192.168.0.110";

    private static readonly string url = "https://" + HostIP + ":8091/get_smth";

    private static readonly List<Color> colors = new List<Color> { Color.green, Color.blue, Color.red, Color.yellow, Color.cyan };

    private List<string> responses = new List<string>();

    ClickManager ClickManager;

    public string MapFeatureToVariable(string FeatureName)
    {
        if (FeatureName == "sepal length (cm)")
        {
            return "sepalLength";
        } else if (FeatureName == "sepal width (cm)")
        {
            return "sepalWidth";
        }
        else if (FeatureName == "petal length (cm)")
        {
            return "petalLength";
        }
        else if (FeatureName == "petal width (cm)")
        {
            return "petalWidth";
        }
        return "";
    }

    public void CreateDataSphereXYZ(GameObject go, DataContainerXYZ rec, Dictionary<string, float> XMinMax, Dictionary<string, float> YMinMax, Dictionary<string, float> ZMinMax, string XComp = "None", string YComp = "None", string ZComp = "None")
    {      
        GameObject DataPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);

        float XCoord, YCoord, ZCoord;

        DataPoint.transform.parent = go.transform;

        //XCoord = (float)rec.GetType().GetField(XComp).GetValue(rec);
        //YCoord = (float)rec.GetType().GetField(YComp).GetValue(rec);
        XCoord = (float)rec.X;
        YCoord = (float)rec.Y;
        ZCoord = (float)rec.Z;

        DataPoint.transform.localPosition = new Vector3((XCoord - XMinMax["min"]) / (XMinMax["max"] - XMinMax["min"]) * 0.8f, (YCoord - YMinMax["min"]) / (YMinMax["max"] - YMinMax["min"]) * 0.8f, (ZCoord - ZMinMax["min"]) / (ZMinMax["max"] - ZMinMax["min"]) * 0.8f);
        DataPoint.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        DataPoint.tag = "datapoint";

        // Save information about each datapoint:
        GameObject XInfo = new GameObject();
        XInfo.name = "XInfo";
        XInfo.transform.parent = DataPoint.transform;
        GameObject XValue = new GameObject();
        XValue.name = XCoord.ToString();
        XValue.transform.parent = XInfo.transform;
        GameObject YInfo = new GameObject();
        YInfo.name = "YInfo";
        YInfo.transform.parent = DataPoint.transform;
        GameObject YValue = new GameObject();
        YValue.name = YCoord.ToString();
        YValue.transform.parent = YInfo.transform;
        GameObject ZInfo = new GameObject();
        ZInfo.name = "ZInfo";
        ZInfo.transform.parent = DataPoint.transform;
        GameObject ZValue = new GameObject();
        ZValue.name = ZCoord.ToString();
        ZValue.transform.parent = ZInfo.transform;

        if (go.transform.parent.parent.parent.Find("kmeansTest").gameObject.tag == "exercise" && rec.Target == -2)
        {
            DataPoint.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            GameObject Info = Instantiate(KmeansExInfo);
            Info.name = "info";
            Info.transform.parent = DataPoint.transform;
            Info.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            Info.transform.localPosition = new Vector3(10f, 10f, 0f);
            Info.AddComponent<LookAtCam>();
            Info.SetActive(false);
        }
        else
        {
            GameObject Info = Instantiate(DatapointInfo);
            Info.name = "info";
            Info.transform.parent = DataPoint.transform;
            Info.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            Info.transform.localPosition = new Vector3(10f, 5f, 0f);
            Info.transform.Find("XComp").GetComponent<TextMeshPro>().text = XComp;
            Info.transform.Find("YComp").GetComponent<TextMeshPro>().text = YComp;
            Info.transform.Find("ZComp").GetComponent<TextMeshPro>().text = ZComp;
            Info.transform.Find("XCompVal").GetComponent<TextMeshPro>().text = XCoord.ToString();
            Info.transform.Find("YCompVal").GetComponent<TextMeshPro>().text = YCoord.ToString();
            Info.transform.Find("ZCompVal").GetComponent<TextMeshPro>().text = ZCoord.ToString();
            Info.transform.Find("ClassVal").GetComponent<TextMeshPro>().text = rec.ClassStr;
            Info.AddComponent<LookAtCam>();
            Info.SetActive(false);

            //DataPoint.AddComponent<DatapointInfo>();
        }

        DataPoint.AddComponent<DatapointInfo>();

        GameObject Index = new GameObject();
        Index.name = "Index";
        Index.transform.parent = DataPoint.transform;
        GameObject IndexValue = new GameObject();
        IndexValue.name = rec.Index.ToString();
        IndexValue.transform.parent = Index.transform;

        GameObject Target = new GameObject();
        Target.name = "Target";
        Target.transform.parent = DataPoint.transform;
        GameObject TargetValue = new GameObject();
        TargetValue.name = rec.Target.ToString();
        TargetValue.transform.parent = Target.transform;

        var DataRenderer = DataPoint.GetComponent<Renderer>();
        if (rec.Target == -1)
        {
            DataRenderer.material.SetColor("_Color", Color.black);
            DataPoint.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
            DataPoint.tag = "centroid";
        }
        else if (rec.Target == -2)
        {
            DataPoint.tag = "toClassify";
        }
        else
        {
            DataRenderer.material.SetColor("_Color", colors[rec.Target]);
        }
        /*if (rec.Target == 0)
        {
            DataRenderer.material.SetColor("_Color", Color.red);
        }
        else if (rec.Target == 1)
        {
            DataRenderer.material.SetColor("_Color", Color.green);
        }
        else if (rec.Target == 2)
        {
            DataRenderer.material.SetColor("_Color", Color.blue);
        }
        else if (rec.Target == -1)
        {
            DataRenderer.material.SetColor("_Color", Color.black);
            DataPoint.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        }*/
    }

    public async Task<bool> LoadDatasetMeta(GameObject go, string DatasetName)
    {
        GameObject Dataset = new List<GameObject>(GameObject.FindGameObjectsWithTag("Dataset")).Find(g => g.transform.IsChildOf(go.transform));
        
        if (DatasetName.Split("!").Count() > 1)
        {
            Dataset.name = DatasetName.Split("!")[0];
            go.transform.Find("kmeansTest").tag = "true";
        }
        else
        {
            Dataset.name = DatasetName;
            go.transform.Find("kmeansTest").tag = "false";
        }

        PythonServerRequest psr = new PythonServerRequest("dataset_meta", Dataset.name);

        StartCoroutine(PostAsyncRequest(url, psr, RequestCallback));

        //string ServerResponse = await SendServerRequest(psr);

        //var VariableList = JsonConvert.DeserializeObject<List<String>>(ServerResponse);

        var VariableListString = await ExtractResponse();

        var VariableList = JsonConvert.DeserializeObject<List<String>>(VariableListString);

        Debug.Log(VariableListString);
        
        int NumberOfVariables = VariableList.Count;

        float ColumnHelper = NumberOfVariables / 5.0f;

        var Columns = Math.Ceiling(ColumnHelper);

        int Position = 1;
        int Column = 1;

        foreach (String VariableName in VariableList)
        {
            go.GetComponent<CreateFeatures>().CreateVariableBox(VariableName, go, Position, Columns, Column);
            Position += 1;
            if (Position == 6)
            {
                Position = 1;
                Column += 1;
            }
        }

        return true;

        //CreateLegend(DatasetName);
    }

    public void ResetDatapoints(GameObject go)
    {
        foreach (Transform datapoint in go.transform.Find("DataPoints3D/Data"))
        {
            if (int.Parse(datapoint.Find("Target").GetChild(0).name) >= 0)
            {
                datapoint.Find("Target").GetChild(0).name = "-2";
                datapoint.GetComponent<Renderer>().material.color = Color.white;
                datapoint.tag = "toClassify";
            }
        }
    }

    public async void UpdateCentroids(GameObject go)
    {
        var datapointTargetDict = new Dictionary<int, int>();

        foreach (Transform datapoint in go.transform.Find("DataPoints3D/Data"))
        {
            datapointTargetDict[int.Parse(datapoint.Find("Index").GetChild(0).name)] = int.Parse(datapoint.Find("Target").GetChild(0).name);
        }

        PythonServerRequest psr;
        psr = new PythonServerRequest("kmeans_ex", "custom_kmeansExercise", componentX: "a", componentY: "b", componentZ: "c", datapointClasses: datapointTargetDict);

        StartCoroutine(PostAsyncRequest(url, psr, RequestCallback));

        var DataFromServer = await ExtractResponse();
        DataFromServer = DataFromServer.Replace("\"[", "[").Replace("]\"", "]").Replace("\\", "");

        Debug.Log(DataFromServer);

        var DataList = JsonConvert.DeserializeObject<List<DataContainerXYZ>>(DataFromServer);

        var XMinMax = new Dictionary<string, float>();
        var YMinMax = new Dictionary<string, float>();
        var ZMinMax = new Dictionary<string, float>();

        XMinMax.Add("min", 0f);
        XMinMax.Add("max", 4f);
        YMinMax.Add("min", 0f);
        YMinMax.Add("max", 4f);
        ZMinMax.Add("min", 0f);
        ZMinMax.Add("max", 4f);

        GameObject DataContainer = go.transform.Find("DataPoints3D/Data").GameObject();

        var Centroids = GameObject.FindGameObjectsWithTag("centroid").OrderByDescending(c => int.Parse(c.transform.Find("Index").GetChild(0).name)).ToList();

        if (DataContainer.transform.childCount > 0)
        {
            foreach (Transform child in DataContainer.transform)
            {
                if (child.tag == "centroid")
                {
                    //GameObject.Destroy(child.gameObject);
                }
            }
        }

        List<Vector3> centroidCoords = new List<Vector3>();
        foreach (DataContainerXYZ rec in DataList)
        {
            centroidCoords.Add(new Vector3((float)rec.X * 0.2f, (float)rec.Y * 0.2f, (float)rec.Z * 0.2f));
            //CreateDataSphereXYZ(DataContainer, rec, XMinMax, YMinMax, ZMinMax, "a", "b", "c");
        }

        go.transform.parent.parent.GetComponent<KmeansExercise>().setCentroidMovement(Centroids, centroidCoords);
    }



    public async void Visualize3DDataFromServerXYZ(GameObject go, string XComp = "XComp", string YComp = "YComp", string ZComp = "ZComp")
    {
        Debug.Log("Visualize3DDataFromServerXYZ started");

        bool kmeans;
        bool exercise = false;
        if (go.transform.Find("kmeansTest").tag == "true")
        {
            kmeans = true;
        } else
        {
            kmeans = false;
            go.transform.Find("VisualizeData(Clone)/kmeansButtons").gameObject.SetActive(false);
        }
        if (go.transform.Find("kmeansTest").tag == "exercise")
        {
            exercise = true;
        }


        GameObject DataCube = go.transform.Find("VisualizeData(Clone)").gameObject;

        DataCube.transform.Find("XCompAxis/AxisName").GetComponent<TextMeshPro>().text = XComp;
        DataCube.transform.Find("YCompAxis/AxisName").GetComponent<TextMeshPro>().text = YComp;
        DataCube.transform.Find("ZCompAxis/AxisName").GetComponent<TextMeshPro>().text = ZComp;

        GameObject DataPoints3D = DataCube.transform.Find("DataPoints3D").gameObject;

        GameObject Axis = DataPoints3D.transform.Find("Axis").gameObject;

        string dataset = new List<GameObject>(GameObject.FindGameObjectsWithTag("Dataset")).Find(g => g.transform.IsChildOf(go.transform)).name;

        if (XComp == "XComp")
        {
            XComp = GameObject.FindGameObjectsWithTag("XComp")[0].name;
        }
        if (YComp == "YComp")
        {
            YComp = GameObject.FindGameObjectsWithTag("YComp")[0].name;
        }
        if (ZComp == "ZComp")
        {
            ZComp = GameObject.FindGameObjectsWithTag("ZComp")[0].name;
        }

        GameObject MarkerContainerX = Axis.transform.Find("MarkerContainer/MarkerContainerX").gameObject;
        GameObject MarkerContainerY = Axis.transform.Find("MarkerContainer/MarkerContainerY").gameObject;
        GameObject MarkerContainerZ = Axis.transform.Find("MarkerContainer/MarkerContainerZ").gameObject;

        Dictionary<string, float> XMinMax, YMinMax, ZMinMax;

        XMinMax = new Dictionary<string, float>();
        YMinMax = new Dictionary<string, float>();
        ZMinMax = new Dictionary<string, float>();

        if (XComp != "XComp")
        {
            XMinMax = await CreateAxis(XComp, dataset, "X", MarkerContainerX.transform.childCount);
        }

        if (YComp != "YComp")
        {
            YMinMax = await CreateAxis(YComp, dataset, "Y", MarkerContainerY.transform.childCount);
        }

        if (ZComp != "ZComp")
        {
            ZMinMax = await CreateAxis(ZComp, dataset, "Z", MarkerContainerZ.transform.childCount);
        }

        if (XComp == "XComp" || YComp == "YComp" || ZComp == "ZComp")
        {
            return;
        }

        PythonServerRequest psr;
        List<DataContainerXYZ> KmeansData = new List<DataContainerXYZ>();

        if (!kmeans)
        {
            if (!exercise)
            {
                await CreateLegend(go);
                psr = new PythonServerRequest("visualize_xyz", dataset, componentX: XComp, componentY: YComp, componentZ: ZComp);
            } else
            {
                psr = new PythonServerRequest("kmeans_ex", dataset, componentX: XComp, componentY: YComp, componentZ: ZComp);
            }
        } else
        {
            psr = new PythonServerRequest("kmeans_xyz", dataset, componentX: XComp, componentY: YComp, componentZ: ZComp);
        }

        //var DataFromServer = await SendServerRequest(psr);

        //DataFromServer = DataFromServer.Replace("\"[", "[").Replace("]\"", "]").Replace("\\", "");

        StartCoroutine(PostAsyncRequest(url, psr, RequestCallback));

        var DataFromServer = await ExtractResponse();
        DataFromServer = DataFromServer.Replace("\"[", "[").Replace("]\"", "]").Replace("\\", "");

        Debug.Log(DataFromServer);

        GameObject DataContainer = DataPoints3D.transform.Find("Data").GameObject();
        if (DataContainer.transform.childCount > 0)
        {
            foreach (Transform child in DataContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        if (!kmeans)
        {
            var DataList = JsonConvert.DeserializeObject<List<DataContainerXYZ>>(DataFromServer);

            foreach (DataContainerXYZ rec in DataList)
            {
                CreateDataSphereXYZ(DataContainer, rec, XMinMax, YMinMax, ZMinMax, XComp, YComp, ZComp);
            }
        } else
        {
            var DataList = JsonConvert.DeserializeObject<List<List<DataContainerXYZ>>>(DataFromServer);
            var last = DataList.Last();
            foreach (var Data in DataList)
            {
                Debug.Log("Next KMeans step...");
                if (Data == last)
                {
                    go.transform.Find("VisualizeData(Clone)/kmeansButtons").gameObject.SetActive(false);
                }

                foreach (var D in Data)
                {
                    KmeansData.Add(D);
                }

                if (DataContainer.transform.childCount > 0)
                {
                    foreach (Transform child in DataContainer.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }

                foreach (DataContainerXYZ rec in KmeansData)
                {
                    CreateDataSphereXYZ(DataContainer, rec, XMinMax, YMinMax, ZMinMax, XComp, YComp, ZComp);
                }

                KmeansData.Clear();

                //if (ClickManager)
                await Task.Delay(TimeSpan.FromSeconds(0.2));
                await NextKmeans();
            }
        }

        Debug.Log("Visualize3DDataFromServerXYZ finished");
    }

    public async Task<bool> NextKmeans()
    {
        bool next = ClickManager.getNext();
        if (next)
        {
            return true;
        } else
        {
            await Task.Delay(TimeSpan.FromSeconds(0.1));
            bool next_n = await NextKmeans();
            return next_n;
        }
    }

    public async Task<bool> CreateLegend(GameObject go)
    {
        //var dataset = GameObject.FindGameObjectWithTag("datasetSelection").name;
        GameObject Dataset = new List<GameObject>(GameObject.FindGameObjectsWithTag("Dataset")).Find(g => g.transform.IsChildOf(go.transform));

        PythonServerRequest psr = new PythonServerRequest("legend", Dataset.name);
        //var DataFromServer = await SendServerRequest(psr);

        StartCoroutine(PostAsyncRequest(url, psr, RequestCallback));

        var DataFromServer = await ExtractResponse();

        var LegendDict = JsonConvert.DeserializeObject<Dictionary<float, string>>(DataFromServer);

        Debug.Log("LegendDict created");
        
        foreach (KeyValuePair<float, string> entry in LegendDict)
        {
            int ind = (int)Math.Round(entry.Key);

            go.transform.Find("VisualizeData(Clone)/Legend/LegendContainer").GetComponent<CreateLegend>().CreateLegendEntry(ind, entry.Value, colors);

            /*string LegendName = "LegendEntry_" + ind.ToString();
            Debug.Log(LegendName);
            GameObject.Find(LegendName).SetActive(true);
            var LegendObj = GameObject.Find(LegendName);
            LegendObj.transform.Find("Sphere").GetComponent<Renderer>().material.SetColor("_Color", colors[ind]);
            LegendObj.transform.Find("TargetName").GetComponent<TextMesh>().text = entry.Value;*/
        }

        return true;
    }

    public async Task<Dictionary<string, float>> CreateAxis(string Comp, string dataset, string AxisDir, int MarkerCount)
    {
        Debug.Log("CreateAxis started");
        GameObject AxisGO;
        if (AxisDir == "X")
        {
            AxisGO = GameObject.Find("Xaxis");
        }
        else if (AxisDir == "Y")
        {
            AxisGO = GameObject.Find("Yaxis");
        }
        else
        {
            AxisGO = GameObject.Find("Zaxis");
        }
        PythonServerRequest psr = new PythonServerRequest("axis", dataset, componentX: Comp);
        //var AxisRaw = await SendServerRequest(psr);
        //Debug.Log(AxisRaw);

        StartCoroutine(PostAsyncRequest(url, psr, RequestCallback));

        var AxisRaw = await ExtractResponse();

        var AxisDeserialized = JsonConvert.DeserializeObject<Dictionary<String, List<float>>>(AxisRaw);

        float AxisStepsSize = 1f / AxisDeserialized["steps"].Count;

        if (MarkerCount == 0)
        {
            for (int i = 1; i <= AxisDeserialized["steps"].Count(); i++)
            {
                AxisGO.GetComponent<CreateAxisMarker>().CreateMarker(AxisDir, AxisStepsSize * i, AxisDeserialized["steps"][i - 1]);
            }
        }

        Dictionary<string, float> ReturnDict = new Dictionary<string, float>();

        ReturnDict.Add("min", AxisDeserialized["min"][0]);
        ReturnDict.Add("max", AxisDeserialized["max"][0]);

        Debug.Log("CreateAxis finished");

        return ReturnDict;
    }

    public async void Kmeans3DServerXYZ(GameObject go)
    {
        string XComp, YComp, ZComp;

        string dataset = GameObject.FindGameObjectWithTag("datasetSelection").name;

        XComp = GameObject.FindGameObjectsWithTag("XComp")[0].name;
        YComp = GameObject.FindGameObjectsWithTag("YComp")[0].name;
        ZComp = GameObject.FindGameObjectsWithTag("ZComp")[0].name;

        GameObject MarkerContainerX = GameObject.Find("MarkerContainerX");
        GameObject MarkerContainerY = GameObject.Find("MarkerContainerY");
        GameObject MarkerContainerZ = GameObject.Find("MarkerContainerZ");

        Dictionary<string, float> XMinMax, YMinMax, ZMinMax;

        XMinMax = new Dictionary<string, float>();
        YMinMax = new Dictionary<string, float>();
        ZMinMax = new Dictionary<string, float>();

        if (XComp != "XComps3D")
        {
            XMinMax = await CreateAxis(XComp, dataset, "X", MarkerContainerX.transform.childCount);
        }

        if (YComp != "YComps3D")
        {
            YMinMax = await CreateAxis(YComp, dataset, "Y", MarkerContainerY.transform.childCount);
        }

        if (ZComp != "ZComps3D")
        {
            ZMinMax = await CreateAxis(ZComp, dataset, "Z", MarkerContainerZ.transform.childCount);
        }

        if (XComp == "XComps3D" || YComp == "YComps3D" || ZComp == "ZComps3D")
        {
            return;
        }

        List<DataContainerXYZ> IrisData = new List<DataContainerXYZ>();

        PythonServerRequest psr = new PythonServerRequest("kmeans_xyz", dataset, componentX: XComp, componentY: YComp, componentZ: ZComp);
        var DataFromServer = await SendServerRequest(psr);
        DataFromServer = DataFromServer.Replace("\"[", "[").Replace("]\"", "]").Replace("\\", "");

        Debug.Log(DataFromServer);

        //var DataFromServer = await GetDataFromServer("iris");

        var DataList = JsonConvert.DeserializeObject<List<List<DataContainerXYZ>>>(DataFromServer);

        foreach (var Data in DataList)
        {
            Debug.Log("Next KMeans step...");
            foreach (var D in Data)
            {
                IrisData.Add(D);
            }


            if (go.transform.childCount > 0)
            {
                foreach (Transform child in go.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }

            foreach (DataContainerXYZ rec in IrisData)
            {
                CreateDataSphereXYZ(go, rec, XMinMax, YMinMax, ZMinMax, "z");
            }

            IrisData.Clear();

            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }



    public void OnDataCubeInitialize()
    {
        Debug.Log("OnDataCubeInitialize started");
        if (m_DataCubeName != "")
        {
            Visualize3DDataFromServerXYZ(GameObject.Find(m_DataCubeName));
        }
        Debug.Log("OnDataCubeInitialize finished");
    }

    public void CreateLog(string logType, string objectClicked = null, Dictionary<int,float> kmeansDistances = null)
    {
        Debug.Log("###############################\n\nCreateLog called");
        LogMessage log = new LogMessage("log", AnalyticsSessionInfo.sessionId.ToString(), DateTime.Now.ToString("yyyyMMddHHmmssffff"), logType, objectClicked, kmeansDistances);
        StartCoroutine(LogRequest(url, log));
    }

    public async void StartUpTest()
    {
        PythonServerRequest psr = new PythonServerRequest("StartUp");
        var DataFromServer = await SendServerRequest(psr);
        Debug.Log(DataFromServer);
    }

    public async Task<string> SendServerRequest(PythonServerRequest psr = null)
    {
        var values = JsonConvert.SerializeObject(psr);

        Debug.Log(values);

        try
        {
            var response = await client.PostAsync(new Uri(url), new StringContent(values, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
        catch (Exception e)
        {
            Debug.Log("Caught exception : " + e);
            Debug.Log("###########################");
            Debug.Log("Message : " + e.InnerException.Message);
            Debug.Log("###########################");

            return "Error";
        }
    }

    private IEnumerator PostAsyncRequest(string urlArg, PythonServerRequest psr, Action<UnityWebRequest> callback)
    {
        Debug.Log("PostAsyncRequest started");
        var values = JsonConvert.SerializeObject(psr);

        Debug.Log(values);

        byte[] jsonToSend = new UTF8Encoding().GetBytes(values);

        //using (UnityWebRequest request = UnityWebRequest.Post(urlArg, values))
        using (UnityWebRequest request = new UnityWebRequest(urlArg, "POST"))
        {
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();

            Debug.Log(request.ToString());

            yield return request.SendWebRequest();

            Debug.Log(request.result);

            callback(request);
            Debug.Log("PostAsyncRequest finished");
        }
    }

    private IEnumerator GetAsyncRequest(string urlArg, Action<UnityWebRequest> callback)
    {
        var request = UnityWebRequest.Get(urlArg);

        Debug.Log("Host URL: " + urlArg);

        request.SetRequestHeader("Content-Type", "application/json");

        request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();

        yield return request.SendWebRequest();

        callback(request);
    }

    private IEnumerator LogRequest(string urlArg, LogMessage log)
    {
        var values = JsonConvert.SerializeObject(log);

        Debug.Log(values);

        byte[] jsonToSend = new UTF8Encoding().GetBytes(values);

        using (UnityWebRequest request = new UnityWebRequest(urlArg, "POST"))
        {
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificPublicKey();

            Debug.Log("###############################\n\nLogRequest:");
            Debug.Log(request.ToString());

            yield return request.SendWebRequest();
        }
    }

    private void RequestCallback(UnityWebRequest request)
    {
        Debug.Log("RequestCallback started");

        Debug.LogFormat("Response Code: {0}\nResponse Text: {1}", request.responseCode, request.downloadHandler.text);
        responses.Add(request.downloadHandler.text);
        //responses[0] = request.downloadHandler.text;

        Debug.Log("RequestCallback finished");
    }

    private void PrintResponses()
    {
        Debug.Log("##### UnityWebRequest #####");
        Debug.Log(responses.Count);
        foreach (var response in responses)
        {
            Debug.Log(response);
        }
        responses.Clear();
    }

    private async Task<string> ExtractResponse()
    {
        Debug.Log("ExtractResponses started");
        /*if (responses.Count == 0)
        {
            //Invoke("ExtractResponse", 1);
            return ExtractResponse();
        }
        else
        {
            string response = responses[0];
            responses.Clear();
            Debug.Log("ExtractResponses finished");
            return response;
        }*/

        while (responses.Count == 0) await Task.Delay(1);
        string response = responses[0];
        responses.Clear();
        Debug.Log("ExtractResponses finished");
        return response;
    }

    // Start is called before the first frame update
    void Start()
    {
        ClickManager = GameObject.Find("ClickManager").GetComponent<ClickManager>();
        //Debug.Log(Application.internetReachability);
        StartCoroutine(GetAsyncRequest(url, RequestCallback));
        //StartUpTest();
        Invoke("PrintResponses", 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
