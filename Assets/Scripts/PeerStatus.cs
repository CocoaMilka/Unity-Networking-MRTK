using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PeerStatus : MonoBehaviour
{
    [SerializeField] TMP_Text statusText;
    string initStatusText;
    // Start is called before the first frame update
    void Start()
    {
        initStatusText = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Peer1Track()
    {        
        statusText.text = initStatusText + "Peer 1 Initialized";
        initStatusText = statusText.text;
    }

    public void Peer2Track()
    {
        statusText.text = initStatusText + "Peer 2 Initialized";
        initStatusText = statusText.text;
    }

    public void Peer3Track()
    {
        statusText.text = initStatusText + "Peer 3 Initialized";
        initStatusText = statusText.text;
    }
}
