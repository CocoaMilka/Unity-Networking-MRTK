using System;
using Unity.BossRoom.UnityServices.Lobbies;
using Unity.BossRoom.Utils;
//using Unity.BossRoom.Infrastructure;
using Unity.Netcode;
using UnityEngine;

//using VContainer;

namespace Unity.BossRoom.ConnectionManagement
{
    /// <summary>
    /// Base class representing a connection state.
    /// </summary>
    abstract class ConnectionState 
    {
        //[Inject]
        protected ConnectionManager m_ConnectionManager => AppController.Instance.ConnectionManager;
        protected LobbyServiceFacade m_LobbyServiceFacade => AppController.Instance.LobbyServiceFacade;
        //[Inject]
        protected ProfileManager m_ProfileManager => AppController.Instance.ProfileManager;
        //[Inject]
        protected LocalLobby m_LocalLobby => AppController.Instance.LocalLobby;

        //[Inject]
        //protected IPublisher<ConnectStatus> m_ConnectStatusPublisher;

        public abstract void Enter();

        public abstract void Exit();

        public virtual void OnClientConnected(ulong clientId) { }
        public virtual void OnClientDisconnect(ulong clientId) { }

        public virtual void OnServerStarted() { }

        public virtual void StartClientIP(string playerName, string ipaddress, int port) { }

        public virtual void StartClientLobby(string playerName) { }

        public virtual void StartHostIP(string playerName, string ipaddress, int port) { }

        public virtual void StartHostLobby(string playerName) { }

        public virtual void OnUserRequestedShutdown() { }

        public virtual void OnDisconnectReasonReceived(ConnectStatus disconnectReason) { }

        public virtual void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) { }

        public virtual void OnTransportFailure() { }
    }
}
