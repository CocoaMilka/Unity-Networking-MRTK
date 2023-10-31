using System;
using Unity.BossRoom.ConnectionManagement;
using Unity.BossRoom.UnityServices.Lobbies;
using Unity.BossRoom.Utils;
using Unity.Multiplayer.Samples.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
//using VContainer;

namespace Unity.BossRoom.ConnectionManagement
{
    /// <summary>
    /// Connection state corresponding to when the NetworkManager is shut down. From this state we can transition to the
    /// ClientConnecting sate, if starting as a client, or the StartingHost state, if starting as a host.
    /// </summary>
    class OfflineState : ConnectionState
    {
        //[Inject]
        //LobbyServiceFacade m_LobbyServiceFacade => AppController.Instance.LobbyServiceFacade;
        ////[Inject]
        //ProfileManager m_ProfileManager;
        ////[Inject]
        //LocalLobby m_LocalLobby;

        const string k_MainMenuSceneName = "MenuSelect";
        const string k_MainMenuSceneNameDesktop = "MenuSelectGoPro";
        const string k_MainMenuSceneNameAll = "MenuSelectAll";

        public override void Enter()
        {
            m_LobbyServiceFacade.EndTracking();
            m_ConnectionManager.NetworkManager.Shutdown();
            //AppController.Instance.LobbyServiceFacade.EndTracking();
            //AppController.Instance.NetworkManager.Shutdown();
            if (SceneManager.GetActiveScene().name != k_MainMenuSceneName)
            {
                //SceneWrapper.Instance.LoadScene(k_MainMenuSceneName, useNetworkSceneManager: false);
                //#if UNITY_STANDALONE
                //                SceneWrapper.Instance.LoadScene(k_MainMenuSceneNameDesktop, useNetworkSceneManager: false);
                //#elif UNITY_EDITOR
                //                SceneWrapper.Instance.LoadScene(k_MainMenuSceneNameDesktop, useNetworkSceneManager: false);
                //#else
                //                SceneWrapper.Instance.LoadScene(k_MainMenuSceneName, useNetworkSceneManager: false);
                //#endif
                SceneWrapper.Instance.LoadScene(k_MainMenuSceneNameAll, useNetworkSceneManager: false);
            }
        }

        public override void Exit() { }

        public override void StartClientIP(string playerName, string ipaddress, int port)
        {
            var connectionMethod = new ConnectionMethodIP(ipaddress, (ushort)port, m_ConnectionManager, m_ProfileManager, playerName);
            m_ConnectionManager.m_ClientReconnecting.Configure(connectionMethod);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientConnecting.Configure(connectionMethod));
        }

        public override void StartClientLobby(string playerName)
        {
            var connectionMethod = new ConnectionMethodRelay(m_LobbyServiceFacade, m_LocalLobby, m_ConnectionManager, m_ProfileManager, playerName);
            m_ConnectionManager.m_ClientReconnecting.Configure(connectionMethod);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_ClientConnecting.Configure(connectionMethod));
        }

        public override void StartHostIP(string playerName, string ipaddress, int port)
        {
            var connectionMethod = new ConnectionMethodIP(ipaddress, (ushort)port, m_ConnectionManager, m_ProfileManager, playerName);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_StartingHost.Configure(connectionMethod));
        }

        public override void StartHostLobby(string playerName)
        {
            var connectionMethod = new ConnectionMethodRelay(m_LobbyServiceFacade, m_LocalLobby, m_ConnectionManager, m_ProfileManager, playerName);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_StartingHost.Configure(connectionMethod));
        }
    }
}
