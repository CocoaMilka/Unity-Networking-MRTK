using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Interactions : NetworkBehaviour
{
    [SerializeField] GameObject ballModel;
    [SerializeField] GameObject robotModel;
    [SerializeField] GameObject droneModel;
    [SerializeField] GameObject bridgeModel;
    [SerializeField] Button robotSpawnButton;
    [SerializeField] Button droneSpawnButton;
    [SerializeField] Button bridgeSpawnButton;
    [SerializeField] Image spawnImageBackground;
    private Vector3 posInitialization;
    private GameObject fireBall;
    private Transform fireBallTransform;
    private GameObject trackRobot, trackDrone, trackBridge;
    public static NetworkVariable<bool> robotSpawned = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public static NetworkVariable<bool> droneSpawned = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public static NetworkVariable<bool> bridgeSpawned = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool trackRobotClt, trackDroneClt, trackBridgeClt = false;
    private bool imageIsInactive = false;
    MeshRenderer[] meshRenderers;
    // Start is called before the first frame update
    void Start()
    {
        posInitialization = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(!imageIsInactive && trackRobot != null && trackDrone != null && trackBridge != null)
        {
            spawnImageBackground.gameObject.SetActive(false);
            imageIsInactive = true;
        }
        /*
        if (robotSpawnButton != null)
        {
            if (robotSpawned.Value && robotSpawnButton.enabled)
            {
                robotSpawnButton.enabled = false;
            }
        }


        if (droneSpawnButton != null)
        {
            if (droneSpawned.Value && droneSpawnButton.enabled)
            {
                droneSpawnButton.enabled = false;
            }
        }

        if (bridgeSpawnButton != null)
        {
            if (bridgeSpawned.Value && bridgeSpawnButton.enabled)
            {
                bridgeSpawnButton.enabled = false;
            }
        }

        */
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //OnBallSpawnServerRPC();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnRobotSpawnServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            OnDroneSpawnServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            OnBridgeSpawnServerRpc();
        }
        /*
        if(robotSpawnButton != null)
        {
            if (robotSpawned.Value && robotSpawnButton.enabled)
            {
                robotSpawnButton.enabled = false;
            }
        }
        

        if(droneSpawnButton != null)
        {
            if (droneSpawned.Value && droneSpawnButton.enabled)
            {
                droneSpawnButton.enabled = false;
            }
        }
        
        if(bridgeSpawnButton != null)
        {
            if (bridgeSpawned.Value && bridgeSpawnButton.enabled)
            {
                bridgeSpawnButton.enabled = false;
            }
        }
        //*/

        if (spawnImageBackground != null)
        {
            if (spawnImageBackground.isActiveAndEnabled)
            {
                if (!robotSpawnButton.enabled && !droneSpawnButton.enabled && !bridgeSpawnButton.enabled)
                {
                    spawnImageBackground.gameObject.SetActive(false); 
                }
            }
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void OnBallSpawnServerRpc()
    {
        posInitialization = gameObject.transform.position;
        posInitialization.y += 1.5f;
        var go = Instantiate(ballModel, posInitialization, Quaternion.identity);
        go.GetComponent<MeshRenderer>().material.color = PlayerSettings.Instance.playerColors[(int)OwnerClientId];
        go.GetComponent<NetworkObject>().Spawn();
        fireBall = go;
        fireBallTransform = fireBall.transform;
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnRobotSpawnServerRpc()
    {        
        posInitialization = gameObject.transform.position;
        posInitialization.x += 0.5f; //1.5f;
        //posInitialization.y += 1.5f;
        posInitialization.z += 1.5f;
        trackRobot = Instantiate(robotModel, posInitialization, Quaternion.identity);
        trackRobot.GetComponent<NetworkObject>().Spawn();
        robotSpawned.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnDroneSpawnServerRpc()
    {
        posInitialization = gameObject.transform.position;
        posInitialization.x += -1.0f;//1.5f;
        posInitialization.y += 0.5f;
        posInitialization.z += 1.0f;
        trackDrone = Instantiate(droneModel, posInitialization, Quaternion.identity);
        trackDrone.GetComponent<NetworkObject>().Spawn();
        droneSpawned.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnBridgeSpawnServerRpc()
    {
        posInitialization = gameObject.transform.position;
        posInitialization.x += -1.0f;//1.5f;
        posInitialization.z += 0.5f;//1.5f;
        trackBridge = Instantiate(bridgeModel, posInitialization, Quaternion.identity);
        meshRenderers = trackBridge.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in meshRenderers)
        {
            renderer.gameObject.AddComponent<MeshCollider>();
        }
        trackBridge.GetComponent<NetworkObject>().Spawn();
        bridgeSpawned.Value = true;
    }

    [ServerRpc(RequireOwnership =false)]

    public void OnSpawnButtonPressedServerRpc()
    {

    }


    public override void OnNetworkDespawn()
    {
        trackRobot?.GetComponent<NetworkObject>().Despawn();
        trackDrone?.GetComponent<NetworkObject>().Despawn();
        trackBridge?.GetComponent<NetworkObject>().Despawn();
        base.OnNetworkDespawn();
    }
    public override void OnDestroy()
    {
        
        Destroy(trackRobot);
        Destroy(trackDrone);
        Destroy(trackBridge);
        base.OnDestroy();
    }

    
}
