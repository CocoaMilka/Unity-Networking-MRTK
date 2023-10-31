using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.BossRoom.UnityServices.Lobbies;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class LobbyUIMediator : MonoBehaviour
{
    //[SerializeField] CanvasGroup m_CanvasGroup;
    //[SerializeField] LobbyJoiningUI m_LobbyJoiningUI;
    //[SerializeField] LobbyCreationUI m_LobbyCreationUI;
    //[SerializeField] UITinter m_JoinToggleHighlight;
    //[SerializeField] UITinter m_JoinToggleTabBlocker;
    //[SerializeField] UITinter m_CreateToggleHighlight;
    //[SerializeField] UITinter m_CreateToggleTabBlocker;
    //[SerializeField] TextMeshProUGUI m_PlayerNameLabel;
    //[SerializeField] GameObject m_LoadingSpinner;

    //AuthenticationServiceFacade m_AuthenticationServiceFacade;
    LobbyServiceFacade m_LobbyServiceFacade => AppController.Instance.LobbyServiceFacade;
    LocalLobbyUser m_LocalUser => AppController.Instance.LocalLobbyUser;
    LocalLobby m_LocalLobby => AppController.Instance.LocalLobby;
    //NameGenerationData m_NameGenerationData;
    ConnectionManager m_ConnectionManager => AppController.Instance.ConnectionManager;
    //ISubscriber<ConnectStatus> m_ConnectStatusSubscriber;

    const string k_DefaultLobbyName = "no-name";
    /*
    //[Inject]
    void InjectDependenciesAndInitialize(
        AuthenticationServiceFacade authenticationServiceFacade,
        LobbyServiceFacade lobbyServiceFacade,
        LocalLobbyUser localUser,
        LocalLobby localLobby,
        NameGenerationData nameGenerationData,
        ISubscriber<ConnectStatus> connectStatusSub,
        ConnectionManager connectionManager
    )
    {
        m_AuthenticationServiceFacade = authenticationServiceFacade;
        m_NameGenerationData = nameGenerationData;
        m_LocalUser = localUser;
        m_LobbyServiceFacade = lobbyServiceFacade;
        m_LocalLobby = localLobby;
        m_ConnectionManager = connectionManager;
        m_ConnectStatusSubscriber = connectStatusSub;
        RegenerateName();

        m_ConnectStatusSubscriber.Subscribe(OnConnectStatus);
    }
    //*/
    void OnConnectStatus(ConnectStatus status)
    {
        /*
        if (status is ConnectStatus.GenericDisconnect or ConnectStatus.StartClientFailed)
        {
            //UnblockUIAfterLoadingIsComplete();
        }
        */
    }

    void OnDestroy()
    {
        //m_ConnectStatusSubscriber?.Unsubscribe(OnConnectStatus);
    }

    //Lobby and Relay calls done from UI

    public async void CreateLobbyRequest(string lobbyName, bool isPrivate)
    {
        // before sending request to lobby service, populate an empty lobby name, if necessary
        if (string.IsNullOrEmpty(lobbyName))
        {
            lobbyName = k_DefaultLobbyName;
        }

        //BlockUIWhileLoadingIsInProgress();

        //bool playerIsAuthorized = await m_AuthenticationServiceFacade.EnsurePlayerIsAuthorized();
        bool playerIsAuthorized = await EnsurePlayerIsAuthorized();
        if (!playerIsAuthorized)
        {
            //UnblockUIAfterLoadingIsComplete();
            return;
        }

        var lobbyCreationAttempt = await m_LobbyServiceFacade.TryCreateLobbyAsync(lobbyName, m_ConnectionManager.MaxConnectedPlayers, isPrivate);

        if (lobbyCreationAttempt.Success)
        {
            m_LocalUser.IsHost = true;
            m_LobbyServiceFacade.SetRemoteLobby(lobbyCreationAttempt.Lobby);

            Debug.Log($"Created lobby with ID: {m_LocalLobby.LobbyID} and code {m_LocalLobby.LobbyCode}");
            m_ConnectionManager.StartHostLobby(m_LocalUser.DisplayName);
        }
        else
        {
            //UnblockUIAfterLoadingIsComplete();
        }
    }

    public async Task<bool> EnsurePlayerIsAuthorized()
    {
        if (AuthenticationService.Instance.IsAuthorized)
        {
            return true;
        }

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            return true;
        }
        catch (AuthenticationException e)
        {
            var reason = $"{e.Message} ({e.InnerException?.Message})";
            //m_UnityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
            //not rethrowing for authentication exceptions - any failure to authenticate is considered "handled failure"
            return false;
        }
        catch (Exception e)
        {
            //all other exceptions should still bubble up as unhandled ones
            var reason = $"{e.Message} ({e.InnerException?.Message})";
            //m_UnityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
            throw;
        }
    }

    public async void QueryLobbiesRequest(bool blockUI)
    {
        if (Unity.Services.Core.UnityServices.State != ServicesInitializationState.Initialized)
        {
            return;
        }

        if (blockUI)
        {
            //BlockUIWhileLoadingIsInProgress();
        }

        //bool playerIsAuthorized = await m_AuthenticationServiceFacade.EnsurePlayerIsAuthorized();
        bool playerIsAuthorized = await EnsurePlayerIsAuthorized();

        if (blockUI && !playerIsAuthorized)
        {
            //UnblockUIAfterLoadingIsComplete();
            return;
        }

        await m_LobbyServiceFacade.RetrieveAndPublishLobbyListAsync();

        if (blockUI)
        {
            //UnblockUIAfterLoadingIsComplete();
        }
    }

    public async void JoinLobbyWithCodeRequest(string lobbyCode)
    {
        //BlockUIWhileLoadingIsInProgress();

        //bool playerIsAuthorized = await m_AuthenticationServiceFacade.EnsurePlayerIsAuthorized();
        bool playerIsAuthorized = await EnsurePlayerIsAuthorized();

        if (!playerIsAuthorized)
        {
            //UnblockUIAfterLoadingIsComplete();
            return;
        }

        var result = await m_LobbyServiceFacade.TryJoinLobbyAsync(null, lobbyCode);

        if (result.Success)
        {
            OnJoinedLobby(result.Lobby);
        }
        else
        {
            //UnblockUIAfterLoadingIsComplete();
        }
    }

    public async void JoinLobbyRequest(LocalLobby lobby)
    {
        //BlockUIWhileLoadingIsInProgress();

        //bool playerIsAuthorized = await m_AuthenticationServiceFacade.EnsurePlayerIsAuthorized();
        bool playerIsAuthorized = await EnsurePlayerIsAuthorized();

        if (!playerIsAuthorized)
        {
            //UnblockUIAfterLoadingIsComplete();
            return;
        }

        var result = await m_LobbyServiceFacade.TryJoinLobbyAsync(lobby.LobbyID, lobby.LobbyCode);

        if (result.Success)
        {
            OnJoinedLobby(result.Lobby);
        }
        else
        {
            //UnblockUIAfterLoadingIsComplete();
        }
    }

    public async void QuickJoinRequest()
    {
        //BlockUIWhileLoadingIsInProgress();

        //bool playerIsAuthorized = await m_AuthenticationServiceFacade.EnsurePlayerIsAuthorized();
        bool playerIsAuthorized = await EnsurePlayerIsAuthorized();

        if (!playerIsAuthorized)
        {
            //UnblockUIAfterLoadingIsComplete();
            return;
        }

        var result = await m_LobbyServiceFacade.TryQuickJoinLobbyAsync();

        if (result.Success)
        {
            OnJoinedLobby(result.Lobby);
        }
        else
        {
            //UnblockUIAfterLoadingIsComplete();
        }
    }

    void OnJoinedLobby(Unity.Services.Lobbies.Models.Lobby remoteLobby)
    {
        m_LobbyServiceFacade.SetRemoteLobby(remoteLobby);

        Debug.Log($"Joined lobby with code: {m_LocalLobby.LobbyCode}, Internal Relay Join Code{m_LocalLobby.RelayJoinCode}");
        m_ConnectionManager.StartClientLobby(m_LocalUser.DisplayName);
    }


}
