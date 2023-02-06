using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogMessage
{
    private string request;
    private string sessionID;
    private string timeStamp;
    private string logType;
    private string objectClicked;

    private Dictionary<int, float> kmeansDistances;

    public string Request { get => request; set => request = value; }
    public string SessionID { get => sessionID; set => sessionID = value; }
    public string TimeStamp { get => timeStamp; set => timeStamp = value; }
    public string LogType { get => logType; set => logType = value; }
    public string ObjectClicked { get => objectClicked; set => objectClicked = value; }

    public Dictionary<int, float> KmeansDistances { get => kmeansDistances; set => kmeansDistances = value; }

    public LogMessage(string request, string sessionID, string timeStamp, string logType, string objectClicked = "None", Dictionary<int, float> kmeansDistances = null)
    {
        this.request = request;
        this.sessionID = sessionID;
        this.timeStamp = timeStamp;
        this.logType = logType;
        this.objectClicked = objectClicked;

        this.kmeansDistances = kmeansDistances;
    }
}
