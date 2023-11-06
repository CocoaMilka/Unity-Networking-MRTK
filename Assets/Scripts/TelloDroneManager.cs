using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TelloDroneManager : MonoBehaviour
{
    private static TelloDroneManager _instance;
    private UdpClient udpClient;
    private Thread commsThread;
    private bool isRunning = true;

    public static TelloDroneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TelloDroneManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = typeof(TelloDroneManager).Name;
                    _instance = go.AddComponent<TelloDroneManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        udpClient = new UdpClient();
        commsThread = new Thread(new ThreadStart(CommsThread));
        commsThread.Start();
    }

    private void CommsThread()
    {
        while (isRunning)
        {
            // TODO: Implement the communication handling here
            // Example: udpClient.Send(...);
            // Example: var receivedResults = udpClient.Receive(ref someEndPoint);
        }
    }

    public void SendCommand(string command)
    {
        byte[] commandBytes = Encoding.UTF8.GetBytes(command);
        udpClient.Send(commandBytes, commandBytes.Length, "192.168.10.1", 8889); // Tello IP and port
        Debug.Log("Command: " + command + " forwarded to drone...!");
    }

    private void OnApplicationQuit()
    {
        isRunning = false;
        if (commsThread != null && commsThread.IsAlive)
            commsThread.Join();

        udpClient?.Close();
        udpClient = null;
    }
}
