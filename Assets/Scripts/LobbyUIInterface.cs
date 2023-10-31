using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyUIInterface : MonoBehaviour
{
    [SerializeField] InputField m_LobbyNameInputField;
    //[SerializeField] GameObject m_LoadingIndicatorObject;
    [SerializeField] Toggle m_IsPrivate;
    //[SerializeField] CanvasGroup m_CanvasGroup;
    ///*[Inject]*/ LobbyUIMediator m_LobbyUIMediator;
    [SerializeField] LobbyUIMediator m_LobbyUIMediator;

    //void Awake()
    //{
    //    EnableUnityRelayUI();
    //}

    //void EnableUnityRelayUI()
    //{
    //    m_LoadingIndicatorObject.SetActive(false);
    //}

    public void OnCreateClick()
    {
        AppController.Instance.Initialize5secUpdate(false);
        m_LobbyUIMediator.CreateLobbyRequest(m_LobbyNameInputField.text, /*m_IsPrivate.isOn*/false);
    }

    //public void Show()
    //{
    //    m_CanvasGroup.alpha = 1f;
    //    m_CanvasGroup.blocksRaycasts = true;
    //}

    //public void Hide()
    //{
    //    m_CanvasGroup.alpha = 0f;
    //    m_CanvasGroup.blocksRaycasts = false;
    //}    

}
