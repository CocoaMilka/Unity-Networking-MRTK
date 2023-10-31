using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour
{
    public void ToggleActiveState()
    {
        if (gameObject != null)
        {
            if (gameObject.activeSelf == true)
                gameObject.SetActive(false);
            else
                gameObject.SetActive(true);
        }
    }
}
