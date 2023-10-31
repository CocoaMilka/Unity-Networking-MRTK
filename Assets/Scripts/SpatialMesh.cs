using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialMesh : MonoBehaviour
{

    void OnEnable()
    {
        var observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
        observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.None;
    }


    private void OnDisable()
    {
        var observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
        observer.DisplayOption = SpatialAwarenessMeshDisplayOptions.Occlusion;
    }

}
