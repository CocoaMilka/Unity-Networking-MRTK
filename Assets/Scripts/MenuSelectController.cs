using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.BossRoom.UnityServices.Lobbies;
using Unity.BossRoom.Utils;
using Unity.Services.Authentication;
using System;
using System.Threading.Tasks;

public class MenuSelectController : NetworkBehaviour
{
    //AuthenticationServiceFacade m_AuthServiceFacade;
    LocalLobbyUser m_LocalUser => AppController.Instance.LocalLobbyUser;
    LocalLobby m_LocalLobby => AppController.Instance.LocalLobby;
    ProfileManager m_ProfileManager => AppController.Instance.ProfileManager;

    protected void Awake()
    {               
        if (string.IsNullOrEmpty(Application.cloudProjectId))
        {            
            return;
        }
        TrySignIn();
    }

    private async void TrySignIn()
    {
        try
        {
            var unityAuthenticationInitOptions = new InitializationOptions();
            var profile = m_ProfileManager.Profile;
            if (profile.Length > 0)
            {
                unityAuthenticationInitOptions.SetProfile(profile);
            }

            //await m_AuthServiceFacade.InitializeAndSignInAsync(unityAuthenticationInitOptions);
            await InitializeAndSignInAsync(unityAuthenticationInitOptions);
            OnAuthSignIn();
            m_ProfileManager.onProfileChanged += OnProfileChanged;
        }
        catch (Exception)
        {
            //OnSignInFailed();
        }
    }

    private void OnAuthSignIn()
    {
        //m_LobbyButton.interactable = true;
        //m_UGSSetupTooltipDetector.enabled = false;
        //m_SignInSpinner.SetActive(false);

        Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");

        m_LocalUser.ID = AuthenticationService.Instance.PlayerId;
        // The local LobbyUser object will be hooked into UI before the LocalLobby is populated during lobby join, so the LocalLobby must know about it already when that happens.
        m_LocalLobby.AddUser(m_LocalUser);
    }

    
    async void OnProfileChanged()
    {
        //m_LobbyButton.interactable = false;
        //m_SignInSpinner.SetActive(true);
        //await m_AuthServiceFacade.SwitchProfileAndReSignInAsync(m_ProfileManager.Profile);
        await SwitchProfileAndReSignInAsync(m_ProfileManager.Profile);
        //m_LobbyButton.interactable = true;
        //m_SignInSpinner.SetActive(false);

        Debug.Log($"Signed in. Unity Player ID {AuthenticationService.Instance.PlayerId}");

        // Updating LocalUser and LocalLobby
        m_LocalLobby.RemoveUser(m_LocalUser);
        m_LocalUser.ID = AuthenticationService.Instance.PlayerId;
        m_LocalLobby.AddUser(m_LocalUser);
    }

    private async Task InitializeAndSignInAsync(InitializationOptions initializationOptions)
    {
        try
        {
            await Unity.Services.Core.UnityServices.InitializeAsync(initializationOptions);

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        catch (Exception e)
        {
            var reason = $"{e.Message} ({e.InnerException?.Message})";
            //m_UnityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
            throw;
        }
    }

    private async Task SwitchProfileAndReSignInAsync(string profile)
    {
        if (AuthenticationService.Instance.IsSignedIn)
        {
            AuthenticationService.Instance.SignOut();
        }
        AuthenticationService.Instance.SwitchProfile(profile);

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception e)
        {
            var reason = $"{e.Message} ({e.InnerException?.Message})";
            //m_UnityServiceErrorMessagePublisher.Publish(new UnityServiceErrorMessage("Authentication Error", reason, UnityServiceErrorMessage.Service.Authentication, e));
            throw;
        }
    }

}
