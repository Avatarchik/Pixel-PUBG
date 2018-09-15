using UnityEngine.Networking;
using UnityEngine;

//网络跟踪
public class NetworkTracer : MonoBehaviour {

	void Start () {
        instance = this;
	}
    private int userbyteTmp;
    private int totalbyteTmp;
    private float timeTmp;
    private int userbytePerSec = 0;
    private int totalbytePerSec = 0;
    public string Detail;
    public static NetworkTracer instance;
    public bool ShowGUI;

    public void Update()
    {
        if (NetworkTransport.IsStarted)
        {
            int userbyte = NetworkTransport.GetOutgoingUserBytesCount();
            int totalbyte = NetworkTransport.GetOutgoingFullBytesCount();

            if (Time.time >= timeTmp + 1)
            {
                userbytePerSec = userbyte - userbyteTmp;
                totalbytePerSec = totalbyte - totalbyteTmp;

                userbyteTmp = userbyte;
                totalbyteTmp = totalbyte;
                timeTmp = Time.time;
            }
            Detail = "Server Total byte (" + totalbyte + "):" + totalbytePerSec + "byte/sec | User total byte (" + userbyte + "):" + userbytePerSec + "byte/sec";
        }
        else
        {
            Detail = "";
        }
    }

    private void OnGUI()
    {
        if (!ShowGUI)
            return;

        GUILayout.Label(Detail);
    }
}
