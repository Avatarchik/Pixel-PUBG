using System.Collections;
using UnityEngine;

public class DebugRenderer : MonoBehaviour {

    public string myLog;
    
    Queue myLogQueue = new Queue();

    void Start () {
        Debug.Log("Debug Renderer enabled.");
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLog = logString;
        string newString = "\n [" + type + "] : " + myLog;
        myLogQueue.Enqueue(newString);
        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            myLogQueue.Enqueue(newString);
        }
        myLog = string.Empty;
        foreach (string mylog in myLogQueue)
        {
            myLog += mylog;
        }
        if (myLogQueue.Count > Screen.height / 25)
            myLogQueue.Dequeue();
    }

    void OnGUI()
    {
        GUILayout.Label(myLog);
    }

}
