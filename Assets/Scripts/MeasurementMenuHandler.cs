using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasurementMenuHandler : MonoBehaviour
{
    public GameObject DistanceComponents;
    public GameObject ButtonBackplate;
    public GameObject MeasurementBackplate;

    public void ShowDistances()
    {
	MeasurementBackplate.SetActive(true);
	ButtonBackplate.SetActive(false);
	DistanceComponents.SetActive(true);
    }

    public void HideDistances()
    {
	MeasurementBackplate.SetActive(false);
	ButtonBackplate.SetActive(true);
	DistanceComponents.SetActive(false);
    }
}
