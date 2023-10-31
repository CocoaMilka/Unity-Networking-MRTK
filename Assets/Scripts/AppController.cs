using System;
using System.Collections;
using System.Collections.Generic;
using Unity.BossRoom.Infrastructure;
using Unity.BossRoom.UnityServices.Lobbies;
using Unity.BossRoom.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppController : MonoBehaviour
{
    [SerializeField] ConnectionManager m_ConnectionManager;
    [SerializeField] NetworkManager m_NetworkManager;
    [SerializeField] LocalLobby m_LocalLobby;
    [SerializeField] LobbyServiceFacade m_LobbyServiceFacade;
    [SerializeField] LocalLobbyUser m_LocalLobbyUser;
    [SerializeField] ProfileManager m_ProfileManager;
    [SerializeField] LobbyAPIInterface m_LobbyAPIInterface;
    IPublisher<LobbyListFetchedMessage> m_LobbyListFetchedPub;
    BufferedMessageChannel<LobbyListFetchedMessage> m_buffPMessage;
    BufferedMessageChannel<LobbyListFetchedMessage> m_buffSMessage;

    public static AppController Instance;
    private bool isUpdating;

    public ConnectionManager ConnectionManager => m_ConnectionManager;
    public NetworkManager NetworkManager => m_NetworkManager;
    public LocalLobby LocalLobby => m_LocalLobby;
    public LobbyServiceFacade LobbyServiceFacade => m_LobbyServiceFacade;
    public LocalLobbyUser LocalLobbyUser => m_LocalLobbyUser;
    public ProfileManager ProfileManager => m_ProfileManager;
    public LobbyAPIInterface LobbyAPIInterface => m_LobbyAPIInterface;
    public IPublisher<LobbyListFetchedMessage> PublishLobbyListFetchedMessage => m_buffPMessage;
    public ISubscriber<LobbyListFetchedMessage> SubscribeLobbyListFetchedMessage => m_buffSMessage;
    //LocalLobby m_LocalLobby;
    //LobbyServiceFacade m_LobbyServiceFacade;
    // Start is called before the first frame update
    private void Start()
    {
        //m_LocalLobby = Container.Resolve<LocalLobby>();
        //m_LobbyServiceFacade = Container.Resolve<LobbyServiceFacade>();

        //m_LocalLobby = new LocalLobby();
        //m_LobbyServiceFacade = new LobbyServiceFacade();

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Application.targetFrameRate = 45;
        
        //SceneManager.LoadScene("MainMenu");
        m_ConnectionManager.ChangeState(m_ConnectionManager.m_Offline);
        m_buffPMessage = new BufferedMessageChannel<LobbyListFetchedMessage>();
        m_buffSMessage = new BufferedMessageChannel<LobbyListFetchedMessage>();
        isUpdating = true;
        Initialize5secUpdate(true);
    }

    //protected override void OnDestroy()
    //{
    //    //m_Subscriptions?.Dispose();
    //    m_LobbyServiceFacade?.EndTracking();
    //    //base.OnDestroy();
    //}

    /// <summary>
    ///     In builds, if we are in a lobby and try to send a Leave request on application quit, it won't go through if we're quitting on the same frame.
    ///     So, we need to delay just briefly to let the request happen (though we don't need to wait for the result).
    /// </summary>
    /// 
    public void Initialize5secUpdate(bool state)
    {
        if (state)
        {
            isUpdating = true;
            StartCoroutine(UpdateEvery5Sec());
        }        
        else
        {
            isUpdating = false;
            StopCoroutine(UpdateEvery5Sec());
        }
    }

    private IEnumerator LeaveBeforeQuit()
    {
        // We want to quit anyways, so if anything happens while trying to leave the Lobby, log the exception then carry on
        try
        {
            m_LobbyServiceFacade.EndTracking();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        yield return null;
        Application.Quit();
    }
    //Changed to 10sec
    private IEnumerator UpdateEvery5Sec()
    {
        while (isUpdating)
        {
            if(LobbyJoiningUI.Instance != null)
            {
                LobbyJoiningUI.Instance.MyPeriodicRefresh();
                Debug.Log("Almost In");
            }
            //yield return new WaitForSeconds(5f);
            yield return new WaitForSeconds(10f);
        }
    }

    private bool OnWantToQuit()
    {
        var canQuit = string.IsNullOrEmpty(m_LocalLobby?.LobbyID);
        if (!canQuit)
        {
            StartCoroutine(LeaveBeforeQuit());
        }
        return canQuit;
    }

    private void OnDestroy()
    {
        isUpdating = false;
        StopCoroutine(UpdateEvery5Sec());
    }
}
