using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.BossRoom.Infrastructure;
using Unity.BossRoom.UnityServices.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class LobbyJoiningUI : MonoBehaviour
{
    [SerializeField]
    InputField m_JoinCodeField;

    [SerializeField]
    Button m_JoinLobbyButton;

    [SerializeField]
    LobbyUIMediator m_LobbyUIMediator;
    public LobbyUIMediator LobbyUIMediator => m_LobbyUIMediator;

    ISubscriber<LobbyListFetchedMessage> m_LocalLobbiesRefreshedSub => AppController.Instance.SubscribeLobbyListFetchedMessage;
    //ISubscriber<LobbyListFetchedMessage> m_LocalLobbiesRefreshedSub;

    List<LobbyListItemUI> m_LobbyListItems = new List<LobbyListItemUI>();

    public static LobbyJoiningUI Instance;
    private void Start()
    {
        Instance = this;
        m_LocalLobbiesRefreshedSub?.Subscribe(UpdateUI);
        //m_LobbyListItems.Add(LobbyListItemUI.Instance);
    }

    public void OnJoinCodeInputTextChanged()
    {
        m_JoinCodeField.text = SanitizeJoinCode(m_JoinCodeField.text);
        m_JoinLobbyButton.interactable = m_JoinCodeField.text.Length > 0;
    }

    string SanitizeJoinCode(string dirtyString)
    {
        return Regex.Replace(dirtyString.ToUpper(), "[^A-Z0-9]", "");
    }

    public void OnJoinButtonPressed()
    {
        m_LobbyUIMediator.JoinLobbyWithCodeRequest(SanitizeJoinCode(m_JoinCodeField.text));
    }

    void PeriodicRefresh(float _)
    {
        //this is a soft refresh without needing to lock the UI and such
        m_LobbyUIMediator.QueryLobbiesRequest(false);
    }

    public void OnRefresh()
    {
        m_LobbyUIMediator.QueryLobbiesRequest(true);
    }

    public void MyPeriodicRefresh()
    {
        m_LobbyUIMediator.QueryLobbiesRequest(false);
    }

    void UpdateUI(LobbyListFetchedMessage message)
    {
        EnsureNumberOfActiveUISlots(message.LocalLobbies.Count);
        Debug.Log($"No of Lobbies fetched: {message.LocalLobbies.Count}");

        for (var i = 0; i < message.LocalLobbies.Count; i++)
        {
            var localLobby = message.LocalLobbies[i];
            m_LobbyListItems[i].SetData(localLobby);            
        }

        if (message.LocalLobbies.Count == 0)
        {
            //m_EmptyLobbyListLabel.enabled = true;
        }
        else
        {
            //m_EmptyLobbyListLabel.enabled = false;
        }
    }

    //Updates the List item directly from the LobbyService Facade without subscription
    public void UpdateUIDirect(LobbyListFetchedMessage message)
    {
        EnsureNumberOfActiveUISlots(message.LocalLobbies.Count);
        Debug.Log($"No of Lobbies fetched: {message.LocalLobbies.Count}");

        //Stop querying for lobbies if found
        if (message.LocalLobbies.Count == 1) 
            AppController.Instance.Initialize5secUpdate(false);        

        for (var i = 0; i < message.LocalLobbies.Count; i++)
        {
            var localLobby = message.LocalLobbies[i];
            //m_LobbyListItems[i].SetData(localLobby);
            LobbyListItemUI.Instance.SetData(localLobby);
        }

        if (message.LocalLobbies.Count == 0)
        {
            //m_EmptyLobbyListLabel.enabled = true;
        }
        else
        {
            //m_EmptyLobbyListLabel.enabled = false;
        }
    }

    void EnsureNumberOfActiveUISlots(int requiredNumber)
    {
        int delta = requiredNumber - m_LobbyListItems.Count;

        for (int i = 0; i < delta; i++)
        {
            //m_LobbyListItems.Add(CreateLobbyListItem());
        }

        for (int i = 0; i < m_LobbyListItems.Count; i++)
        {
            m_LobbyListItems[i].gameObject.SetActive(i < requiredNumber);
        }
    }

    //LobbyListItemUI CreateLobbyListItem()
    //{
    //    var listItem = Instantiate(m_LobbyListItemPrototype.gameObject, m_LobbyListItemPrototype.transform.parent)
    //        .GetComponent<LobbyListItemUI>();
    //    listItem.gameObject.SetActive(true);

    //    m_Container.Inject(listItem);

    //    return listItem;
    //}

    public void OnQuickJoinClicked()
    {
        m_LobbyUIMediator.QuickJoinRequest();
    }
    private void OnDestroy()
    {        
        m_LocalLobbiesRefreshedSub?.Unsubscribe(UpdateUI);
    }
}
