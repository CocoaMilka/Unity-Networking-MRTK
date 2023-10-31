using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScaler : MonoBehaviour
{
    //Stores the normal scale of the bridge prior to making it 1/50th scale
    private Vector3 m_prevScale;
    //Stores the normal position of the bridge prior to making it 1/50th scale
    private Vector3 m_prevPosition;

    public GameObject _MainCamera;

    // Start is called before the first frame update
    void Start()
    {
        m_prevScale = transform.localScale;
        m_prevPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScaleOneToFifty()
    {
        //Save the current scale of the bridge so that it can be restored later
        m_prevScale = gameObject.transform.localScale;
        //Change the scale of the bridge to be 1/50 of the original size
        UpdateScale(m_prevScale/50.0f);
        //Store the current position of the bridge so that it can be restored later
        m_prevPosition = gameObject.transform.position;
        //Move the bridge to the position of the main camera so it is easily visible to the user
        gameObject.transform.position = _MainCamera.transform.position;
    }

    public void ScaleOneToOne()
    {
        //Change the scale of the bridge to go back to its original size
        UpdateScale(m_prevScale);
        //Move the bridge back to where it was before it was made small
        gameObject.transform.position = m_prevPosition;
    }

    void UpdateScale(Vector3 newScale)
    {
        //Save the current scale of the gameobject
        m_prevScale = gameObject.transform.localScale;
        //Update the new scale of the gameobject
        gameObject.transform.localScale = newScale;
    }
}
