using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.BossRoom.UnityServices.Lobbies;
using UnityEngine;

public class LobbyListItemUI : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI m_lobbyNameText;
    //[SerializeField] TextMeshProUGUI m_lobbyCountText;
    [SerializeField] TMP_InputField m_lobbyNameText;
    [SerializeField] TMP_InputField m_lobbyCountText;

    LobbyUIMediator m_LobbyUIMediator => LobbyJoiningUI.Instance.LobbyUIMediator;

    LocalLobby m_Data;
    string l_LobbyName;

    public static LobbyListItemUI Instance;
    private void Start()
    {
        Instance = this;
        l_LobbyName = "";
    }

    public void SetData(LocalLobby data)
    {
        m_Data = data;
        //m_lobbyNameText.SetText(data.LobbyName);
        //m_lobbyCountText.SetText($"{data.PlayerCount}/{data.MaxPlayerCount}");
        m_lobbyNameText.text = data.LobbyName;
        m_lobbyCountText.text = $"{data.PlayerCount}/{data.MaxPlayerCount}";
        Debug.Log(data.LobbyName);

        //Automate client joining the server
        l_LobbyName = data.LobbyName;        
        if (l_LobbyName[0] == 'V' && l_LobbyName[3] == 'k' && l_LobbyName.Length == 8)
        {
            OnClick();
        }
    }

    public void OnClick()
    {
        //AppController.Instance.Initialize5secUpdate(true);
        m_LobbyUIMediator.JoinLobbyRequest(m_Data);
    }
}
