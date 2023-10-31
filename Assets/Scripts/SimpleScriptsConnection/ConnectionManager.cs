using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
//using VContainer;
using System;
using Unity.BossRoom.ConnectionManagement;


public enum ConnectStatus
{
    Undefined,
    Success,                  //client successfully connected. This may also be a successful reconnect.
    ServerFull,               //can't join, server is already at capacity.
    LoggedInAgain,            //logged in on a separate client, causing this one to be kicked out.
    UserRequestedDisconnect,  //Intentional Disconnect triggered by the user.
    GenericDisconnect,        //server disconnected, but no specific reason given.
    Reconnecting,             //client lost connection and is attempting to reconnect.
    IncompatibleBuildType,    //client build type is incompatible with server.
    HostEndedSession,         //host intentionally ended the session.
    StartHostFailed,          // server failed to bind
    StartClientFailed         // failed to connect to server and/or invalid network endpoint
}

//public struct ReconnectMessage
//{
//    public int CurrentAttempt;
//    public int MaxAttempt;

//    public ReconnectMessage(int currentAttempt, int maxAttempt)
//    {
//        CurrentAttempt = currentAttempt;
//        MaxAttempt = maxAttempt;
//    }
//}

//public struct ConnectionEventMessage : INetworkSerializeByMemcpy
//{
//    public ConnectStatus ConnectStatus;
//    public FixedPlayerName PlayerName;
//}

[Serializable]
public class ConnectionPayload
{
    public string playerId;
    public string playerName;
    public bool isDebug;
}

public class ConnectionManager : MonoBehaviour
{
    ConnectionState m_CurrentState;
    //[Inject]
    //[SerializeField] NetworkManager m_NetworkManager;
    //public NetworkManager NetworkManager => m_NetworkManager;
    public NetworkManager NetworkManager => AppController.Instance.NetworkManager;

    [SerializeField]
    int m_NbReconnectAttempts = 2;

    public int NbReconnectAttempts => m_NbReconnectAttempts;

    public int MaxConnectedPlayers = 8;

    internal readonly OfflineState m_Offline = new OfflineState();
    internal readonly ClientConnectingState m_ClientConnecting = new ClientConnectingState();
    internal readonly ClientConnectedState m_ClientConnected = new ClientConnectedState();
    internal readonly ClientReconnectingState m_ClientReconnecting = new ClientReconnectingState();
    internal readonly DisconnectingWithReasonState m_DisconnectingWithReason = new DisconnectingWithReasonState();
    internal readonly StartingHostState m_StartingHost = new StartingHostState();
    internal readonly HostingState m_Hosting = new HostingState();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        List<ConnectionState> states = new() { m_Offline, m_ClientConnecting, m_ClientConnected, m_ClientReconnecting, m_DisconnectingWithReason, m_StartingHost, m_Hosting };
        
        //foreach (var connectionState in states)
        //{
        //    m_Resolver.Inject(connectionState);
        //}

        m_CurrentState = m_Offline;

        //NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
        //NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
        //NetworkManager.OnServerStarted += OnServerStarted;
        //NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
        //NetworkManager.OnTransportFailure += OnTransportFailure;
        //m_CurrentState.Enter();
    }

    internal void ChangeState(ConnectionState nextState)
    {
        //Debug.Log($"{name}: Changed connection state from {m_CurrentState.GetType().Name} to {nextState.GetType().Name}.");

        if (m_CurrentState != null)
        {
            m_CurrentState.Exit();
        }
        m_CurrentState = nextState;
        m_CurrentState.Enter();
    }

    public void StartClientLobby(string playerName)
    {
        m_CurrentState.StartClientLobby(playerName);
    }

    public void StartClientIp(string playerName, string ipaddress, int port)
    {
        m_CurrentState.StartClientIP(playerName, ipaddress, port);
    }

    public void StartHostLobby(string playerName)
    {
        m_CurrentState.StartHostLobby(playerName);
    }

    public void StartHostIp(string playerName, string ipaddress, int port)
    {
        m_CurrentState.StartHostIP(playerName, ipaddress, port);
    }
}
