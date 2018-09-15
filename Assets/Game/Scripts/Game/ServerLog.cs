using System.Collections;
using UnityEngine;

//服务器日志
public class ServerLog : MonoBehaviour
{
    public static ServerLog instance;
    string log;
    string headerLog;
    Queue myLogQueue = new Queue();
    public float linesize = 20;

    void Start()
    {
        instance = this;
    }

    public void Log(string text)
    {
        myLogQueue.Enqueue(text);
        log = string.Empty;

        foreach (string mylog in myLogQueue)
        {
            log += mylog + "\n";
        }
        if (myLogQueue.Count > (Screen.height / linesize) - 2)
            myLogQueue.Dequeue();
    }

    private void OnGUI()
    {
        if (NetworkTracer.instance != null)
            headerLog = NetworkTracer.instance.Detail;

        GUILayout.Label(headerLog+"\n"+log);
    }
}
