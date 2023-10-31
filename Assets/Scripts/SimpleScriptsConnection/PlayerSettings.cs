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
    private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>("Player: 0", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        networkPlayerName.Value = "";// $"Inspector {OwnerClientId + 1}";
        nameText.text = networkPlayerName.Value.ToString();
        meshRendererPlayer.material.color = playerColors[(int)OwnerClientId];
        //transform.position += new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));        
    }
}
