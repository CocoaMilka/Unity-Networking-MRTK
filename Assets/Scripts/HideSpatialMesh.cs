using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSpatialMesh : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
        observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
