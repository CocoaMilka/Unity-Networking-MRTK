using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnStuff : NetworkBehaviour
{
    [SerializeField] GameObject droneModel;
    //private bool trackDroneClt = false;
    private GameObject trackDrone; //Holds reference to the spawned gameObject
    private Vector3 posInitialization; //Initial Position of the gameObject to be spawned
    private int trackBeforeClick; //Ensures only one event executes
    private int trackAfterClick; //Ensures only one event executes
    public static NetworkVariable<bool> droneSpawned = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private bool rendererState; //Tracks the renderer local state
    //private bool clickEnter;
    // Start is called before the first frame update
    void Start()
    {
        posInitialization = new Vector3(0, 0, 0);
        trackBeforeClick = 0;
        trackAfterClick = 0;
        rendererState = false;
        //clickEnter = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Alternate implementation
        //if(clickEnter)
        //{
        //    OnDroneSpawnServerRpc(trackBeforeClick);
        //    Debug.Log(trackBeforeClick);
        //    clickEnter = false;
        //}
    }

    //Toggle's the cube's renderer on and off on all clients
    public void ToggleDroneOnOff()
    {
        trackBeforeClick++;
        OnDroneSpawnServerRpc(trackBeforeClick);
        //Debug.Log(trackBeforeClick);
        //clickEnter = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnDroneSpawnServerRpc(int b4TrackVal)
    {
        OnDroneSpawnClientRpc(b4TrackVal);
        //Debug.Log("Server stuff: $");
    }

    [ClientRpc]
    private void OnDroneSpawnClientRpc(int b4TrackVal)
    {
        if (trackAfterClick != b4TrackVal)
        {
            trackAfterClick = b4TrackVal;
            trackBeforeClick = b4TrackVal;
            //Debug.Log("Client stuff: $");
            if (trackDrone == null)
            {
                //posInitialization = gameObject.transform.position;
                //posInitialization.x += -1.0f;//1.5f;
                //posInitialization.y += 0.5f;
                posInitialization.z += 1.0f;
                trackDrone = Instantiate(droneModel, posInitialization, Quaternion.identity);
                trackDrone.GetComponent<NetworkObject>().Spawn();
                droneSpawned.Value = true;
                rendererState = true;
            }
            else
            {
                trackDrone.GetComponent<Renderer>().enabled = !rendererState;
                rendererState = !rendererState;
            }
        }

        if (b4TrackVal > 200)
        {
            trackAfterClick = 0;
            trackBeforeClick = 0;
        }

    }


}
