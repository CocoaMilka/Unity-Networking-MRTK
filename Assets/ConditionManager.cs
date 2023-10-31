using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;

public class ConditionManager : MonoBehaviour
{
    // condition line item to spawn
    public GameObject ConditionLineItem = null;

    // text from SaveConditionMenu input fields, assigned in editor
    public GameObject ConditionInputFieldText = null;
    public GameObject LocationInputFieldText = null;
    public GameObject SeverityInputFieldText = null;

    public void AddNewCondition()
    {
        // spawn another ConditionLineItem
        var newCondition = Instantiate(ConditionLineItem, gameObject.transform);

        // update ConditionLineItems values to match what was just input in SaveConditionMenu
        // FIX!: hard coded in based on ConditionLineItem prefab, needs to be changed.
        newCondition.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = ConditionInputFieldText.GetComponent<TextMeshProUGUI>().text;
        newCondition.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().text = LocationInputFieldText.GetComponent<TextMeshProUGUI>().text;
        newCondition.transform.GetChild(0).GetChild(2).GetComponent<TextMeshPro>().text = SeverityInputFieldText.GetComponent<TextMeshProUGUI>().text;

        // update the layout of the CondtitionLineItems
        gameObject.GetComponent<GridObjectCollection>().UpdateCollection();
    }
}
