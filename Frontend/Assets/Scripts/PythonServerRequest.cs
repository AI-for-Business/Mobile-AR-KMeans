using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PythonServerRequest
{
    private string request;
    private string dataset;
    private float pointX;
    private float pointY;
    private string componentX;
    private string componentY;
    private string componentZ;

    private string test;
    private Dictionary<int, int> datapointClasses;

    public string Request { get => request; set => request = value; }
    public string Dataset { get => dataset; set => dataset = value; }
    public float PointX { get => pointX; set => pointX = value; }
    public float PointY { get => pointY; set => pointY = value; }
    public string ComponentX { get => componentX; set => componentX = value; }
    public string ComponentY { get => componentY; set => componentY = value; }
    public string ComponentZ { get => componentZ; set => componentZ = value; }
    public Dictionary<int, int> DatapointClasses { get => datapointClasses; set => datapointClasses = value; }

    public PythonServerRequest(string request, string dataset = null, float pointX = 0, float pointY = 0, string componentX = null, string componentY = null, string componentZ = null, Dictionary<int, int> datapointClasses = null)
    {
        this.request = request;
        this.dataset = dataset;
        this.pointX = pointX;
        this.pointY = pointY;
        this.componentX = componentX;
        this.componentY = componentY;
        this.componentZ = componentZ;
        this.datapointClasses = datapointClasses;
    }
}