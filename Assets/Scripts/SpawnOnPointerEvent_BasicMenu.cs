using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

/// <summary>
/// Copied from PointerResult MRTK example
/// Goal is to spawn a prefab'd POI onto a bridge when clicked that
/// will contain info on what the problem is.
/// 
/// Future plans:
/// On click, POI prefab would still be created, but could also create
/// a new data file on the device.  The prefab would then be looking
/// into that file, which could be uploaded to a central database once
/// back in the office. (Might be redundant)
/// </summary>

// namespace needed??

// Example script that spawns a prefab at the pointer hit location.
[AddComponentMenu("Scripts/BasicMenuTesting/SpawnOnPointerEvent_BasicMenu")]
public class SpawnOnPointerEvent_BasicMenu : MonoBehaviour
{
  public GameObject PrefabToSpawn;

  public void Spawn(MixedRealityPointerEventData eventData)
  {
	if (PrefabToSpawn != null)
	{
	  var result = eventData.Pointer.Result;
	  GameObject newPOI = Instantiate(PrefabToSpawn, result.Details.Point, Quaternion.LookRotation(result.Details.Normal));
	  newPOI.name = "POI1";
	}
  }
}