using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{

public class ShowHide_RetrieveMenus : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Gameobject for menu containing the retrieve menu button.")]
    private GameObject RetrieveMenus = null;

    [SerializeField]
    [Tooltip("Gameobjects for all UI windows.")]
    private GameObject[] UIWindows;

    public GameObject[] ButtonToggleState;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Update_Visibility", 5f, 5f);

	ButtonToggleState = new GameObject[UIWindows.Length];
	for(int i = 0; i < UIWindows.Length; i++)
	{
	    ButtonToggleState[i] = UIWindows[i].transform.Find("ButtonPin").Find("BackPlateToggleState").gameObject;
	}
    }

    //Update visibility of the retrieve menus gameobject
    public void Update_Visibility()
    {
	    bool Enable_Visibility = false;
	    for(int i = 0; i < UIWindows.Length; i++)
	    {
	        if(!UIWindows[i].gameObject.GetComponent<RadialView>().enabled)
	        {
		    Enable_Visibility = true;
		    ButtonToggleState[i].SetActive(true);
	        }
		else
		{
		    ButtonToggleState[i].SetActive(false);
		}
	    }
	    RetrieveMenus.gameObject.SetActive(Enable_Visibility);
    }
}
}