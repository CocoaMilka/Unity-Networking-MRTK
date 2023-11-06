using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayerSettings : NetworkBehaviour
{
    [SerializeField] private MeshRenderer meshRendererPlayer;
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private TextMeshProUGUI messageText;
    private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>("Player: 0", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Contains message to send to server
    public NetworkVariable<FixedString128Bytes> message = new NetworkVariable<FixedString128Bytes>("blank", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    public List<Color> playerColors = new List<Color>();

    public static PlayerSettings Instance;

    //private void Awake()
    //{
    //    meshRendererPlayer = GetComponent<MeshRenderer>();
    //}
    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        // Check for key press and if we're the owner of this object
        if (IsOwner && Input.GetKeyDown(KeyCode.K))
        {
            sendCommandToDroneServerRpc("takeoff");
        }

        if (IsOwner && Input.GetKeyDown(KeyCode.L))
        {
            sendCommandToDroneServerRpc("land");
        }

        if (IsOwner && Input.GetKeyDown(KeyCode.J))
        {
            sendCommandToDroneServerRpc("command");
        }
    }

    [ServerRpc]
    void GenerateAndSetRandomNumberServerRpc()
    {
        int randomNumber = Random.Range(0, 100); // Generate a random number
        string randomMessage = $"Random Number: {randomNumber}";
        message.Value = new FixedString128Bytes(randomMessage); // Server sets the value
        UpdateClientUIClientRpc(randomMessage); // Invoke ClientRpc to update UI on all clients
    }

    [ServerRpc]
    void sendCommandToDroneServerRpc(string command)
    {
        TelloDroneManager.Instance.SendCommand(command);
        Debug.Log("Command: " + command + " sent to Server...");
    }

    [ClientRpc]
    void UpdateClientUIClientRpc(string randomMessage)
    {
        if (messageText != null)
        {
            messageText.text = randomMessage; // Update the UI Text element on all clients
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        networkPlayerName.Value = "";// $"Inspector {OwnerClientId + 1}";
        nameText.text = networkPlayerName.Value.ToString();
        meshRendererPlayer.material.color = playerColors[(int)OwnerClientId];
        //transform.position += new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));        
    }
}
