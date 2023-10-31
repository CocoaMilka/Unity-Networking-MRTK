using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SceneWrapper : NetworkBehaviour
{
    bool IsNetworkSceneManagementEnabled => NetworkManager != null && NetworkManager.SceneManager != null && NetworkManager.NetworkConfig.EnableSceneManagement;

    public static SceneWrapper Instance { get; protected set; }

    public virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }

    public virtual void LoadScene(string sceneName, bool useNetworkSceneManager, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        if (useNetworkSceneManager)
        {
            if (IsSpawned && IsNetworkSceneManagementEnabled && !NetworkManager.ShutdownInProgress)
            {
                if (NetworkManager.IsServer)
                {
                    // If is active server and NetworkManager uses scene management, load scene using NetworkManager's SceneManager
                    NetworkManager.SceneManager.LoadScene(sceneName, loadSceneMode);
                }
            }
        }
        else
        {
            // Load using SceneManager
            var loadOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);            
        }
    }

}

