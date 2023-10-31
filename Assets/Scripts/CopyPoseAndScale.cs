using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPoseAndScale : MonoBehaviour
{
    public Transform TransformToCopy= null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // copy the transform position, scale, and rotation of the tracked object transform
        if(TransformToCopy != null)
        {
            //gameObject.transform.position = TransformToCopy.position;
            gameObject.transform.localScale = TransformToCopy.localScale;
            gameObject.transform.rotation = TransformToCopy.rotation;
        }
    }
}
