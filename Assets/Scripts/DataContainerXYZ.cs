using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataContainerXYZ
{
    private string classStr;
    private int target;
    private double x;
    private double y;
    private double z;
    private int index;

    public double X { get => x; set => x = value; }
    public double Y { get => y; set => y = value; }
    public double Z { get => z; set => z = value; }
    public string ClassStr { get => classStr; set => classStr = value; }
    public int Target { get => target; set => target = (int)value; }
    public int Index { get => index; set => index = value; }

    public DataContainerXYZ(double _x, double _y, double _z, int _classInt, string _classStr, int _index)
    {
        x = _x;
        y = _y;
        z = _z;
        target = _classInt;
        classStr = _classStr;
        index = _index;
    }

}